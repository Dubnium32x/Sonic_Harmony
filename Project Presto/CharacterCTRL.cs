using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using UnityEngine;
using UnityEngine.UI;

public class CharacterCTRL : MonoBehaviour {
    CharacterCTRL characterController;

    private bool jump = false;
    public CharControlMotor.SonicState sonicState = CharControlMotor.SonicState.Normal;

    public float AddForce = 20;
    public float jumpForce = 30;
    [SerializeField] private int maxCharge = 7; // Maximum speed for Spindash
    [SerializeField] private float peelHoldtime = 0.8f;
    [SerializeField] private float maxSpeed = 25f;
    [SerializeField] private float startSpinSpeed = 11f;
    [SerializeField] private float maxSpinSpeed = 30f;
    [SerializeField] private float peelSpeed = 40f;
    private float IdleTime = 5.0f;
    private float IdleTimer;
    private bool Impatient = false;
    public bool CanIdle = true;
    public Vector3 moveDirection = Vector3.zero;
    //new variables :
    public float accelerationTime = 2.25f;
    public float decelerationTime = 5; 
    public float turnAroundTime = 0.25f;
    private float minSpeed = 0.05f;
    private float spinSpeed = 0;
    private float time;
    public Transform groundCheck;
    public float gravity = 12f;
    [HideInInspector] public float deceleration = 40;
    [HideInInspector] public float acceleration = 5;
    [HideInInspector] public float turnAroundSpeed = 50;
    //other conditions
    private bool grounded = false;
    private Rigidbody rb3D;
    private bool lifeCheck = true;
    private Animator anim;
    private SpriteRenderer spriteRend;
    private bool flipX;
    public bool jumped;
    private Transform audioSources;
    private AudioSource jumpSource;
    private float jumpFrames = 0.25f;
    private float jumpFramesPassed;

    private bool lookingUp = false;

    private bool lookingDown = false;

    public float speedvar;

    public float verticalValue;
    public float horizontalValue;
    private bool InLoop;

    private float ChargeNumber;
    private float Zvalue;
    private float OrginalZ;
    private Vector3 ScaleValue = new Vector3(10,10,0);

    private AudioSource musicSource;

    private bool MusicEnabled = false;

    public bool GotHurtCheck;
    private bool turnAround;
    public bool Death;
    public bool RingGotB;
    public enum MonitorSpecial { None, Rings10}

    public (float IntitalValue, Vector3 FinalValue, float TotalDistance, bool IsLooping) LoopExitZ;

    private bool JumpButtonState;
    private readonly Queue<Ring> lostRingsPool = new Queue<Ring>();

    [Header("Scriptables")]
    public PlayerStats stats;
    public SonicSfx audios;
    public LevelSettings mysettings;

    [Header("Components")]
    public PlayerInput input;
    public PlayerParticles particles;
    public GameObject lostRing;

    private PlayerShields shield;
    public CharStateMachine state;

    public bool attacking { get; set; }
    public bool halfGravity { get; set; }
    public bool invincible { get; set; }

    public float invincibleTimer { get; set; }
    public int direction { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        mysettings = transform.root.gameObject.GetComponent<LevelSettings>();
        musicSource = GetComponent<AudioSource>();
        OrginalZ = transform.position.z;
        Zvalue = OrginalZ;
        characterController = GetComponent<CharacterCTRL>();
        time = 0;
        rb3D = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        spriteRend = anim.GetComponent<SpriteRenderer>();
        anim.SetFloat("Speed", Mathf.Abs(moveDirection.x));
        anim.SetBool("OnGround", grounded);
        anim.SetBool("Jump", jumped);
        anim.SetBool("Impatient", Impatient);
        audioSources = transform.Find("Audio");
        anim.SetBool("Gothurt",GotHurtCheck);
        anim.SetBool("Death",Death);

        acceleration = maxSpeed / accelerationTime;
        deceleration = maxSpeed / decelerationTime;
        turnAroundSpeed = maxSpeed / turnAroundTime;
        Debug.Log(acceleration + "," + deceleration + "," + turnAroundSpeed);
        Debug.Log(acceleration * Time.deltaTime + "," + deceleration * Time.deltaTime + "," + turnAroundSpeed * Time.deltaTime);
        InitializeLostRingPool();
    }

    // Update is called once per frame
    void Update()
    {
        if (!mysettings.LevelHasLoaded) return;
        if (!MusicEnabled)
        {
            musicSource.Play();
            MusicEnabled = true;
        }
        verticalValue = Input.GetAxisRaw("Vertical");
        horizontalValue = Input.GetAxisRaw("Horizontal");
        JumpButtonState = Input.GetButton("Jump");


    }

    private void FixedUpdate()
    {
        moveDirection = rb3D.velocity;
        speedvar = Mathf.Abs(moveDirection.x);
        grounded = Physics.Linecast(transform.position, groundCheck.position);
        //HandleZ();
        Accell2();
        ImpatientCheck();
        Looking();
        ChargeSpinAndPeel();
        StateChange();
        JumpHandling();
        Looping();
        rb3D.velocity = moveDirection;
        time += Time.deltaTime;
        spriteRend.flipX = flipX;
        anim.SetFloat("Speed", speedvar);
        anim.SetBool("OnGround", grounded);
        anim.SetBool("Jump", jumped);
        anim.SetBool("Impatient", Impatient);
        anim.SetBool("LookUp", lookingUp);
        anim.SetBool("LookDown", lookingDown);
        anim.SetBool("Gothurt", GotHurtCheck);
        anim.SetInteger("SonicState", (int)sonicState);
        anim.SetInteger("Charge", Mathf.RoundToInt(ChargeNumber));
        anim.SetBool("TurnAround", turnAround);
    }

    private void StateChange()
    {
        switch (sonicState)
        {
            case CharControlMotor.SonicState.Normal:
                if(grounded && !jumped)
                {
                    if (Mathf.Abs(moveDirection.magnitude) > maxSpeed + minSpeed)
                    {
                        sonicState = CharControlMotor.SonicState.Peel;
                    }
                    else if (Input.GetAxisRaw("Vertical") < 0 && moveDirection.magnitude > minSpeed)
                    {
                        sonicState = CharControlMotor.SonicState.Spindash;
                    }
                }
                break;
            case CharControlMotor.SonicState.Peel:
                if (Mathf.Abs(moveDirection.magnitude) <= maxSpeed)
                    sonicState = CharControlMotor.SonicState.Normal;
                break;
            case CharControlMotor.SonicState.Spindash:
                if (Mathf.Abs(moveDirection.magnitude) <= minSpeed)
                    sonicState = CharControlMotor.SonicState.Normal;
                break;
            case CharControlMotor.SonicState.SpinningAir:
                if (grounded)
                    sonicState = CharControlMotor.SonicState.Normal;
                break;
        }
    }
    private void Accell2()
    {
        turnAround = false;
        if (Mathf.Abs(horizontalValue) > 0.01 && sonicState != CharControlMotor.SonicState.Spindash && !lookingDown && !lookingUp)
        {
            var speed = horizontalValue;
            if (!Mathf.Sign(moveDirection.x).Equals(Mathf.Sign(speed)))
            {
                turnAround = true;
                speed *= turnAroundSpeed * Time.deltaTime;
            }
            else
            {
                speed *= acceleration * Time.deltaTime;
            }

            moveDirection += InternalMovementHandler(speed);
            moveDirection.x = Mathf.Clamp(moveDirection.x, -maxSpeed, maxSpeed);
            moveDirection.y = Mathf.Clamp(moveDirection.y, -maxSpeed, maxSpeed);
            flipX = moveDirection.x < 0;
        }
        else
        {
            if (Mathf.Abs(moveDirection.y) < minSpeed + deceleration * Time.deltaTime)
            {
                moveDirection.y = 0;
            }
            if (Mathf.Abs(moveDirection.x) < minSpeed + deceleration * Time.deltaTime)
            {
                moveDirection.x = 0;
            }
            else
            {
                moveDirection.x = (Mathf.Abs(moveDirection.x) - deceleration * Time.deltaTime) *
                                  Mathf.Sign(moveDirection.x);
                if (!characterController.grounded) return;

                moveDirection.y = (Mathf.Abs(moveDirection.y) - deceleration * Time.deltaTime) *
                                  Mathf.Sign(moveDirection.y);
            }
        }
    }
    private Vector3 InternalMovementHandler(float speed)
    {
        var realmovement = transform.right * speed;
        realmovement.z = 0;
        return realmovement;
    }
    private void JumpHandling()
    {
        if (sonicState == CharControlMotor.SonicState.ChargingPeel || sonicState == CharControlMotor.SonicState.ChargingSpin)
            return;

        if (characterController.grounded)
        {
            jumpFramesPassed = jumpFrames;
            jumped = false;
            //we are grounded, so recalculate
            //move direction directly from axes

            //moveDirection.y = 0;

            if (!JumpButtonState) return;
            if (!CanIdle) return;
            jumped = true;
            moveDirection.y = jumpForce;
            //jumpSource.Play();
            jumpFramesPassed = 0;
            sonicState = CharControlMotor.SonicState.SpinningAir;
        }
        else
        {
            if (JumpButtonState && jumpFramesPassed < jumpFrames)
            {
                jumpFramesPassed += Time.deltaTime;
                moveDirection.y = jumpForce;
            }
            else
            {
                jumpFramesPassed = jumpFrames;
            }

            if (!characterController.grounded)
            {
                moveDirection.y -= gravity * Time.deltaTime;
            }
        }
    }

    private void ImpatientCheck()
    {
        if (verticalValue.CompareTo(0f) != 0)
        {
            Impatient = false;
            return;
        }
        if (speedvar <= 1f && grounded && !Impatient && CanIdle)
        {
            IdleTimer += Time.deltaTime;
            if (!(IdleTimer > IdleTime)) return;
            // Remove the recorded seconds
            IdleTimer -= IdleTime;
            Impatient = true;
        }
        else if(speedvar > 1f || !grounded)
        {
            Impatient = false;
        }
        else if (speedvar <= 1f && grounded && Impatient)
        {
            IdleTimer += Time.deltaTime;
            if (!(IdleTimer > IdleTime)) return;

            // Remove the recorded seconds
            IdleTimer -= IdleTime;
            Impatient = false;
        }
    }

    private void Looking()
    {
        if (speedvar >= 1f || !grounded)
        {
            lookingDown = false;
            lookingUp = false;
            return;
        }
        if (verticalValue >= 1)
        {
            lookingUp = true;
            lookingDown = false;
        }
        else if (verticalValue < 0)
        {
            lookingUp = false;
            lookingDown = true;
        }
        else
        {
            lookingUp = false;
            lookingDown = false;
        }
    }

    public void RingGot()
    {
        RingGotB = true;
        transform.root.gameObject.GetComponent<LevelSettings>().RingAdd();
    }

    public void GotHurt(bool fatal)
    {
        if (!fatal)
        {
            GotHurtCheck = true;
            var mydirection = (moveDirection * -1);
            rb3D.AddForce( mydirection,ForceMode.VelocityChange);
        }
        else
        {
            GotHurtCheck = true;
            Death = true;
        }
        transform.root.gameObject.GetComponent<LevelSettings>().GotHurtL(fatal);
        GotHurtCheck = false;
    }
    public void ChargeSpinAndPeel()
    {
        if (!grounded)
            return;

        if (lookingDown)
        {
            if (JumpButtonState)
            {
                sonicState = CharControlMotor.SonicState.ChargingSpin;

                // Don't exceed max speed
                if (ChargeNumber < maxCharge)
                {
                    ChargeNumber++;
                    // Default spindash speed on first press
                    if (ChargeNumber == 1)
                        spinSpeed = startSpinSpeed;
                    // Increase speed each press
                    else
                    {
                        spinSpeed += (maxSpinSpeed - startSpinSpeed) / maxCharge;
                    }
                }
            }
        }
        else if(lookingUp)
        {
            if (Input.GetButton("Jump"))
            {
                sonicState = CharControlMotor.SonicState.ChargingPeel;
                
                // Cap the speed
                if (ChargeNumber > maxCharge)
                    ChargeNumber = maxCharge;
                else if (ChargeNumber < maxCharge)
                    ChargeNumber += (maxCharge / peelHoldtime) * Time.deltaTime;
            }
        }
        else if (sonicState == CharControlMotor.SonicState.ChargingSpin && !lookingDown)
        {
            sonicState = CharControlMotor.SonicState.Spindash;
            moveDirection.x = transform.right.x * (spinSpeed * (flipX ? -1 : 1));
            moveDirection.y = transform.right.y * (spinSpeed * (flipX ? -1 : 1));
            spinSpeed = 0;
            ChargeNumber = 0;
        }
        else if (sonicState == CharControlMotor.SonicState.ChargingPeel && (!JumpButtonState || !lookingUp))
        {
            sonicState = CharControlMotor.SonicState.Normal;
            moveDirection.x = transform.right.x * (peelSpeed * (flipX ? -1 : 1) * (ChargeNumber / maxCharge));
            moveDirection.y = transform.right.y * (peelSpeed * (flipX ? -1 : 1) * (ChargeNumber / maxCharge));
            ChargeNumber = 0;
        }
    }

    public void SetZlayer(float zvalue)
    {
        var tmpPosition = transform.position;
        tmpPosition.z = zvalue;
        transform.position = tmpPosition;
    }

    public void Looping()
    {

        if (!LoopExitZ.IsLooping)
        {
            return;
        }
        var value = transform.position.z;

        var distance = (new Vector2(LoopExitZ.FinalValue.x, LoopExitZ.FinalValue.y) - new Vector2(transform.position.x,transform.position.y)).sqrMagnitude;
        var FractionalValue = Mathf.Abs(distance / LoopExitZ.TotalDistance);
        Debug.Log(FractionalValue);
        //if getting further away
        if (FractionalValue > 1.0f)
        {
            value = Mathf.Lerp(transform.position.z, LoopExitZ.IntitalValue, FractionalValue);
            if (Math.Abs((value - LoopExitZ.IntitalValue)/ LoopExitZ.IntitalValue) < 0.5f)
            {
                LoopExitZ.IsLooping = false;
                SetZlayer(LoopExitZ.IntitalValue);
                return;
            }
        }
        //if getting closer
        else if (FractionalValue < 1.0f)
        {
            
            value = Mathf.Lerp(LoopExitZ.IntitalValue, LoopExitZ.FinalValue.z, FractionalValue);
            if (Math.Abs((value - LoopExitZ.FinalValue.z)/ LoopExitZ.FinalValue.z) < float.Epsilon)
            {
                LoopExitZ.IsLooping = false;
                SetZlayer(LoopExitZ.FinalValue.z);
                return;
            }
        } else if (Math.Abs(FractionalValue - 1) < float.Epsilon)
        {
            LoopExitZ.IsLooping = false;
            SetZlayer(value);
            return;
        }
        SetZlayer(value);
    }

    public void SetLoopExitZ(Vector3 FinalValue)
    {
        var TotalDistance = (new Vector2(LoopExitZ.FinalValue.x, LoopExitZ.FinalValue.y) - new Vector2(transform.position.x, transform.position.y)).sqrMagnitude;
        //if reached exit point exit code;
        if (LoopExitZ.IsLooping)
        {
            LoopExitZ.IsLooping = false;
        }
        LoopExitZ = (IntitalValue:transform.position.z, FinalValue,TotalDistance, IsLooping:true);
    }
    public void ActivateSpecial(MonitorSpecial myEnum)
    {
        throw new NotImplementedException();
    }

    private void InitializeLostRingPool()
    {
        for (int i = 0; i < stats.maxLostRingCount; i++)
        {
            var gameObject = Instantiate(lostRing);

            if (gameObject.TryGetComponent(out Ring ring))
            {
                ring.Disable();
                lostRingsPool.Enqueue(ring);
            }
        }
    }

    private Ring InstantiateLostRing(Vector3 position, Quaternion rotation)
    {
        var ring = lostRingsPool.Dequeue();
        ring.transform.SetPositionAndRotation(position, rotation);
        ring.Enable();
        lostRingsPool.Enqueue(ring);
        return ring;
    }

    private void ScatterRings(float amount)
    {
        var angle = 101.25f;
        var flipDirection = false;
        var force = stats.ringScatterForce;

        amount = Mathf.Min(amount, stats.maxLostRingCount);

        for (int i = 0; i < amount; i++)
        {
            var ring = InstantiateLostRing(transform.position, Quaternion.identity);
            ring.velocity.y = Mathf.Sin(angle) * force;
            ring.velocity.x = Mathf.Cos(angle) * force;

            if (flipDirection)
            {
                ring.velocity.x *= -1;
                angle += 22.5f;
            }

            flipDirection = !flipDirection;

            if (i == 16)
            {
                force *= 0.5f;
                angle = 101.25f;
            }
        }
    }
    public void ApplyHurt(Vector3 hurtPoint)
    {
        if (!invincible)
        {
            if (shield != PlayerShields.None || mysettings.Rings > 0)
            {
                moveDirection.y = stats.pushBack;
                moveDirection.x = stats.pushBack * 0.5f * Mathf.Sign(transform.position.x - hurtPoint.x);
                state.ChangeState<HurtPlayerState>();

                if (shield == PlayerShields.None)
                {
                    jumpSource.PlayOneShot(audios.ring_loss, 0.4f);
                    ScatterRings(mysettings.Rings);
                    mysettings.Rings = 0;
                }
                else
                {
                    SetShield(PlayerShields.None);
                    jumpSource.PlayOneShot(audios.death);
                }
            }
            else
            {
                ApplyDeath();
            }
        }
    }

    public void ApplyDeath()
    {


        SetShield(PlayerShields.None);
        state.ChangeState<DiePlayerState>();
        transform.root.rotation = Quaternion.Euler(0, 90, 0);
        mysettings.NoRings();
    }
    public void SetShield(PlayerShields shield)
    {
        this.shield = shield;

        switch (shield)
        {
            case PlayerShields.None:
                particles.normalShield.Stop();
                break;
            case PlayerShields.Normal:
                particles.normalShield.Play();
                break;
        }
    }
}