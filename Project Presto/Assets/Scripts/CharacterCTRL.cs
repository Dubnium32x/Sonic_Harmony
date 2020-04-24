using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterCTRL : MonoBehaviour {
    CharacterCTRL characterController;

    private bool jump = false;
    public SonicState sonicState = SonicState.Normal;

    public float AddForce = 20;
    public float jumpForce = 30;
    [SerializeField] private int maxCharge = 7;
    [SerializeField] private float peelHoldtime = 0.8f;
    [SerializeField] private float maxSpeed = 25f;
    [SerializeField] private float spinSpeed = 30f;
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

    private bool InLoop;

    private float ChargeNumber;
    private float Zvalue;
    private float OrginalZ;
    private Vector3 ScaleValue = new Vector3(10,10,0);
    private LevelSettings mysettings;

    private AudioSource musicSource;

    private bool MusicEnabled = false;

    public bool GotHurtCheck;
    private bool turnAround;
    public bool Death;
    public bool RingGotB;
    public enum SonicState { Normal, ChargingSpin, Spindash, ChargingPeel, Peel, SpinningAir, Damaged, Dead };
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
        jumpSource = audioSources.Find("Jump").GetComponent<AudioSource>();
        anim.SetBool("Gothurt",GotHurtCheck);
        anim.SetBool("Death",Death);

        acceleration = maxSpeed / accelerationTime;
        deceleration = maxSpeed / decelerationTime;
        turnAroundSpeed = maxSpeed / turnAroundTime;
        Debug.Log(acceleration + "," + deceleration + "," + turnAroundSpeed);
        Debug.Log(acceleration * Time.deltaTime + "," + deceleration * Time.deltaTime + "," + turnAroundSpeed * Time.deltaTime);
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
        moveDirection = rb3D.velocity;
        speedvar = Mathf.Abs(moveDirection.x);
        verticalValue = Input.GetAxisRaw("Vertical");
        grounded = Physics.Linecast(transform.position, groundCheck.position);
        HandleZ();
        Accell2();
        ImpatientCheck();
        Looking();
        ChargeSpinAndPeel();
        StateChange();
        JumpHandling();

        rb3D.velocity = moveDirection;
        time += Time.deltaTime;
        spriteRend.flipX = flipX;
        anim.SetFloat("Speed", speedvar);
        anim.SetBool("OnGround", grounded);
        anim.SetBool("Jump", jumped);
        anim.SetBool("Impatient", Impatient);
        anim.SetBool("LookUp",lookingUp);
        anim.SetBool("LookDown",lookingDown);
        anim.SetBool("Gothurt", GotHurtCheck);
        anim.SetInteger("SonicState", (int)sonicState);
        anim.SetInteger("Charge", Mathf.RoundToInt(ChargeNumber));
        anim.SetBool("TurnAround", turnAround);
    }

    private void StateChange()
    {
        switch (sonicState)
        {
            case SonicState.Normal:
                if(grounded)
                {
                    if (Mathf.Abs(moveDirection.magnitude) > maxSpeed + minSpeed)
                        sonicState = SonicState.Peel;
                    else if (Input.GetAxisRaw("Vertical") < 0 && moveDirection.magnitude > minSpeed)
                        sonicState = SonicState.Spindash;
                }
                break;
            case SonicState.Peel:
                if (Mathf.Abs(moveDirection.magnitude) <= maxSpeed)
                    sonicState = SonicState.Normal;
                break;
            case SonicState.Spindash:
                if (Mathf.Abs(moveDirection.magnitude) <= minSpeed)
                    sonicState = SonicState.Normal;
                break;
            case SonicState.SpinningAir:
                if (grounded)
                    sonicState = SonicState.Normal;
                break;
        }
    }
    private void HandleZ()
    {
        var pos = transform.position;
        if (pos.z != Zvalue)
        {
            pos.z = Zvalue;
            transform.position = pos;
        }
    }
    private void Accell2()
    {
        turnAround = false;
        if (Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0.01 && sonicState != SonicState.Spindash && !lookingDown && !lookingUp)
        {
            float speed = Input.GetAxisRaw("Horizontal");
            if (!Mathf.Sign(moveDirection.x).Equals(Mathf.Sign(speed)))
            {
                turnAround = true;
                speed *= turnAroundSpeed * Time.deltaTime;
            }
            else
                speed *= acceleration * Time.deltaTime;
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
        if (sonicState == SonicState.ChargingPeel || sonicState == SonicState.ChargingSpin)
            return;

        if (characterController.grounded)
        {
            jumpFramesPassed = jumpFrames;
            jumped = false;
            //we are grounded, so recalculate
            //move direction directly from axes

            //moveDirection.y = 0;

            if (!Input.GetButton("Jump")) return;
            if (!CanIdle) return;
            jumped = true;
            moveDirection.y = jumpForce;
            jumpSource.Play();
            jumpFramesPassed = 0;
            sonicState = SonicState.SpinningAir;
        }
        else
        {
            if (Input.GetButton("Jump") && jumpFramesPassed < jumpFrames)
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
            if (Input.GetButtonDown("Jump"))
            {
                sonicState = SonicState.ChargingSpin;
                ChargeNumber += 1;
            }
        }
        else if(lookingUp)
        {
            if (Input.GetButton("Jump"))
            {
                sonicState = SonicState.ChargingPeel;
                ChargeNumber += (maxCharge / peelHoldtime) * Time.deltaTime;
            }
        }
        else if (sonicState == SonicState.ChargingSpin && !lookingDown)
        {
            sonicState = SonicState.Spindash;
            moveDirection.x = spinSpeed * (flipX ? -1 : 1) * (ChargeNumber / maxCharge);
            ChargeNumber = 0;
        }
        else if (sonicState == SonicState.ChargingPeel && (!Input.GetButton("Jump") || !lookingUp))
        {
            sonicState = SonicState.Normal;
            moveDirection.x = peelSpeed * (flipX ? -1 : 1) * (ChargeNumber / maxCharge);
            ChargeNumber = 0;
        }
    }

    public void SetZlayer(float zvalue)
    {
        Zvalue = zvalue;
    }

    public void ResetZvalue()
    {
        Zvalue = OrginalZ;
    }
}