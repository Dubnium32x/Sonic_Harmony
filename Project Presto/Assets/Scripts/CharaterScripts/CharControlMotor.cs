using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharControlMotor : PlayerMotor
{
    // Start is called before the first frame update
    CharControlMotor characterController;

    private bool jump = false;
    public SonicState sonicState = CharControlMotor.SonicState.Normal;


    public bool CanIdle = true;
    //new variables :

    private float minSpeed = 0.05f;
    private float spinSpeed = 0;
    private float time;
    [HideInInspector] public float deceleration = 40;
    [HideInInspector] public float acceleration = 5;
    [HideInInspector] public float turnAroundSpeed = 50;
    //other conditions
    private bool lifeCheck = true;
    private Animator anim;
    private bool flipX;
    public bool jumped;
    private Transform audioSources;
    public AudioSource jumpSource;
    private float jumpFrames = 0.25f;
    private float jumpFramesPassed;

    public bool lookingUp = false;

    public bool lookingDown = false;

    public float speedvar;

    private bool InLoop;

    private float ChargeNumber;
    private Vector3 ScaleValue = new Vector3(10, 10, 0);

    private AudioSource musicSource;

    private bool MusicEnabled = false;

    public bool GotHurtCheck;
    private bool turnAround;
    public bool Death;
    public bool RingGotB;
    public enum SonicState
    {
        Normal = 0,
        Crouch = 1,
        Dead = 2,
        Damaged = 3,
        Jump = 4,
        LookUp = 5,
        Rolling = 6,
        Spindash = 7,
        Spring = 8,
        Brake = 9,
        SpinningAir = 10,
        Peel = 11,
        ChargingPeel = 12,
        ChargingSpin = 13,
        LedgeGrabFront = 14,
        LedgeGrabBack = 15,
        Push = 16
    };
    //public enum SonicState { ChargingSpin, Damaged, Dead, Brake, Jump, Rolling, LookUp, Crouch, Spring, Walk };

    private bool JumpButtonState;
    private readonly Queue<Ring> lostRingsPool = new Queue<Ring>();

    [Header("Scriptables")]
    public PlayerStats stats;
    public SonicSfx audios;

    [Header("Components")]
    public PlayerInput input;
    public PlayerParticles particles;
    public GameObject lostRing;
    public new CamFollow camera;
    public PlayerSkin skin;

    private PlayerShields shield;
    public CharStateMachine state;

    public bool attacking { get; set; }
    public bool downPressed { get; set; }
    public bool halfGravity { get; set; }
    public bool invincible { get; set; }
    
    public bool SpeedShoe { get; set; }
    public float invincibleTimer { get; set; }
    public int direction { get; private set; }

    [Header("Settings")]
    public bool disableInput;
    public bool disableSkinRotation;
    public bool disableCameraFollow;

    protected override void OnMotorUpdate()
    {
        // To whoever put this in the update method:
        // Initializations go into the start method. They should only instantiate things once.

        //InitializeStateMachine();
        //InitializeAudio();
        //InitializeSkin();
        //InitializeAnimatorHash();

        //for some reason this endlessly spawns rings
        //InitializeLostRingPool();
    }
    protected override void OnMotorFixedUpdate(float deltaTime)
    {
        if (!disableInput)
        {
            input.InputUpdate();
            input.UnlockHorizontalControl(deltaTime);

            // Down press bool needed for animation states - Arcy
            downPressed = input.down;
        }

        UpdateInvincibility(deltaTime);
        UpdateSpeedShoe(deltaTime);
        state.UpdateState(deltaTime);
        ClampVelocity();
        ClampToStageBounds();
    }

    protected override void OnMotorLateUpdate()
    {
        UpdateSkinTransform();
        UpdateSkinAnimaiton();
    }

    protected override void OnMotorStart()
    {
        musicSource = GetComponent<AudioSource>();
        characterController = GetComponent<CharControlMotor>();
        time = 0;
        anim = GetComponentInChildren<Animator>();
        //spriteRend = anim.GetComponent<SpriteRenderer>();
        anim.SetBool("OnGround", grounded);
        anim.SetBool("Jump", jumped);
        audioSources = transform.Find("Audio");
        anim.SetBool("Gothurt", GotHurtCheck);
        anim.SetBool("Death", Death);

        Debug.Log(acceleration + "," + deceleration + "," + turnAroundSpeed);
        Debug.Log(acceleration * Time.deltaTime + "," + deceleration * Time.deltaTime + "," + turnAroundSpeed * Time.deltaTime);

        InitializeStateMachine();
        InitializeSkin();
        InitializeLostRingPool();
    }

    private void InitializeLostRingPool()
    {
        for (var i = 0; i < stats.maxLostRingCount; i++)
        {
            var mgameObject = Instantiate(lostRing);

            if (!mgameObject.TryGetComponent(out Ring ring)) continue;
            ring.Disable();
            lostRingsPool.Enqueue(ring);
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
        var mangle = 101.25f;
        var flipDirection = false;
        var force = stats.ringScatterForce;

        amount = Mathf.Min(amount, stats.maxLostRingCount);

        for (var i = 0; i < amount; i++)
        {
            var ring = InstantiateLostRing(transform.position, Quaternion.identity);
            ring.velocity.y = Mathf.Sin(mangle) * force;
            ring.velocity.x = Mathf.Cos(mangle) * force;

            if (flipDirection)
            {
                ring.velocity.x *= -1;
                mangle += 22.5f;
            }

            flipDirection = !flipDirection;

            if (i != 16) continue;
            force *= 0.5f;
            mangle = 101.25f;
        }
    }
    public void ApplyHurt(Vector3 hurtPoint)
    {
        if (invincible) return;
        if (shield != PlayerShields.None || ScoreManager.Instance.Rings > 0)
        {
            velocity.y = stats.pushBack;
            velocity.x = stats.pushBack * 0.5f * Mathf.Sign(transform.position.x - hurtPoint.x);
            state.ChangeState<HurtPlayerState>();

            if (shield == PlayerShields.None)
            {
                jumpSource.PlayOneShot(audios.ring_loss, 0.4f);
                ScatterRings(ScoreManager.Instance.Rings);
                ScoreManager.Instance.Rings = 0;
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

    public void ApplyDeath()
    {
        var scoreManager = ScoreManager.Instance;

        if (scoreManager)
        {
            scoreManager.Die();
        }

        SetShield(PlayerShields.None);
        state.ChangeState<DiePlayerState>();
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
    public void HandleSlopeFactor(float deltaTime)
    {
        if (!grounded) return;
        if (!attacking)
        {
            velocity.x += up.x * stats.slope * deltaTime;
        }
        else
        {
            var downHill = (Mathf.Sign(velocity.x) == Mathf.Sign(up.x));
            var slope = downHill ? stats.slopeRollDown : stats.slopeRollUp;
            velocity.x += up.x * slope * deltaTime;
        }
    }

    public void HandleAcceleration(float deltaTime)
    {
        var realtopspeed = SpeedShoe ? stats.topSpeed * 2 : stats.topSpeed;
        var acceleration = grounded ? stats.acceleration : stats.airAcceleration;

        if (input.right && (velocity.x < realtopspeed))
        {
            velocity.x += acceleration * deltaTime;
            velocity.x = Mathf.Min(velocity.x, realtopspeed);
        }
        else if (input.left && (velocity.x > -realtopspeed))
        {
            velocity.x -= acceleration * deltaTime;
            velocity.x = Mathf.Max(velocity.x, -realtopspeed);
        }
    }

    public void HandleDeceleration(float deltaTime)
    {
        if (!grounded) return;
        var deceleration = attacking ? stats.rollDeceleration : stats.deceleration;

        if (input.right && (velocity.x < 0))
        {
            velocity.x += deceleration * deltaTime;

            if (velocity.x >= 0)
            {
                velocity.x = stats.turnSpeed;
            }
        }
        else if (input.left && (velocity.x > 0))
        {
            velocity.x -= deceleration * deltaTime;

            if (velocity.x <= 0)
            {
                velocity.x = -stats.turnSpeed;
            }
        }
    }

    public (bool, bool, bool) HandleLedgeCheck()
    {
        if (!grounded) return (false, false, false);
        if (Mathf.Abs(velocity.x) >= 1.0f) return (false, false, false);
        var thecollider = skin.skin.GetComponent<BoxCollider>();
        var pt1 =  thecollider.bounds.center;
        var pt2 =  thecollider.bounds.center;
        pt1.x -= thecollider.bounds.extents.x;
        pt2.x += thecollider.bounds.extents.x;
         if (skin.skin.transform.rotation.y == 0)
         {
             var tmp = pt1;
             pt1 = pt2;
             pt2 = tmp;
         }
        var LandFoundFront = Physics.Raycast(pt1, -skin.transform.up, thecollider.bounds.extents.y);
        var LandFoundBack = Physics.Raycast(pt2, -skin.transform.up, thecollider.bounds.extents.y);

        return (!LandFoundFront,!LandFoundBack,!LandFoundFront||!LandFoundBack);
    }

    public (bool, GameObject gameObject) HandlePushCheck()
    {
        if (Mathf.Abs(velocity.x) >= 2.0f) return (false,null);
        if (!Physics.Raycast(position, skin.skin.transform.right, out var myhit, 1.0f)) return (false,null);
        
        var myangle = Vector3.Angle(myhit.normal, skin.skin.transform.right);

        return (myangle > 170.0f,myhit.collider.gameObject);
    }
    public void HandleFriction(float deltaTime)
    {
        if (!grounded || (!attacking && (input.horizontal != 0))) return;
        var friction = attacking ? stats.rollFriction : stats.friction;
        velocity = Vector3.MoveTowards(velocity, Vector3.zero, friction * deltaTime);
    }

    public void HandleGravity(float deltaTime)
    {
        if (grounded) return;
        var gravity = halfGravity ? (stats.gravity * 0.5f) : stats.gravity;
        velocity.y -= gravity * deltaTime;
    }

    public void HandleJump()
    {
        if (!grounded) return;
        PlayAudio(audios.jump, 0.4f);
        velocity.y = stats.maxJumpHeight;
        state.ChangeState<JumpPlayerState>();
    }

    public void HandleFall()
    {
        if (!grounded) return;
        if ((!(Mathf.Abs(velocity.x) < stats.minSpeedToSlide)) || (!(angle >= stats.minAngleToSlide))) return;
        if (angle >= stats.minAngleToFall)
        {
            GroundExit();
        }

        input.LockHorizontalControl(stats.controlLockTime);
    }
    public void PlayAudio(AudioClip clip, float volume = 1f)
    {
        jumpSource.Stop();
        jumpSource.PlayOneShot(clip, volume);
    }
    private void ClampVelocity()
    {
        velocity = Vector3.ClampMagnitude(velocity, stats.maxSpeed);
    }
    public void UpdateDirection(float direction)
    {
        if (direction != 0)
        {
            this.direction = (direction > 0) ? 1 : -1;
        }
    }

    public void UpdateInvincibility(float deltaTime)
    {
        if (!invincible || (!(invincibleTimer > 0))) return;
        invincibleTimer -= deltaTime;
        if(!particles.Invinciblity.isEmitting) particles.Invinciblity.Play();
        if (!(invincibleTimer <= 0)) return;
        invincible = false;
        invincibleTimer = 0;
        particles.Invinciblity.Stop();
    }
    public void UpdateSpeedShoe(float deltaTime)
    {
        if (!SpeedShoe || (!(invincibleTimer > 0))) return;
        invincibleTimer -= deltaTime;

        if (!(invincibleTimer <= 0)) return;
        SpeedShoe = false;
        invincibleTimer = 0;
    }
    public void Respawn(Vector3 position, Quaternion rotation)
    {
        direction = 1;
        EnableCollision(true);
        velocity = Vector3.zero;
        disableSkinRotation = disableCameraFollow = false;
        transform.SetPositionAndRotation(position, rotation);
        state.ChangeState<WalkPlayerState>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ForwardTrigger"))
        {
            groundLayer |= (1 << 11);
            groundLayer &= ~(1 << 10);
            wallLayer |= (1 << 11);
            wallLayer &= ~(1 << 10);
        }
        else if (other.CompareTag("BackwardTrigger"))
        {
            groundLayer |= (1 << 10);
            groundLayer &= ~(1 << 11);
            wallLayer |= (1 << 10);
            wallLayer &= ~(1 << 11);
        }
    }
    private void InitializeStateMachine()
    {
        state = new CharStateMachine(this);

        foreach (var mstate in GetComponents<PlayerState>())
        {
            this.state.AddState(mstate);
        }

        state.ChangeState<WalkPlayerState>();
    }
    private void UpdateSkinAnimaiton()
    {
        // Flip sonic's sprite depending on which way he is going
        //ChangeDirection();

        //animator.SetFloat(animationSpeedHash, Mathf.Lerp(0.8f, 3, velocity.magnitude / stats.maxSpeed));

        // Update animation states
        Debug.Log(state.stateId);
        skin.animator.SetInteger("SonicState", (int)state.stateId);
        skin.animator.SetFloat("Speed", Mathf.Abs(velocity.x));
        skin.animator.SetBool("OnGround", grounded);
        skin.animator.SetBool("Jump", jumped);
        //anim.SetBool("Impatient", Impatient); - State not set up yet
        skin.animator.SetBool("Gothurt", GotHurtCheck);
        skin.animator.SetBool("Death", Death);
        skin.animator.SetBool("LookUp", lookingUp);
        skin.animator.SetBool("LookDown", lookingDown);
        skin.animator.SetBool("DownPressed", downPressed);

        /// - Arcy
        var animSpeed = Mathf.Abs(velocity.x); // Check Sonic's speed and convert to anim speed
        if (animSpeed <= 5.0f) animSpeed = 1.0f; // Speed 1
        else if (animSpeed <= 8.0f) animSpeed = 1.5f; // Speed 2
        else animSpeed = 2.0f;// Speed 3

        skin.animator.SetFloat("AnimSpeedMultiplier", animSpeed);
    }

    private void UpdateSkinTransform()
    {
        var yRotation = 90f - direction * 90f;
        float zRotation;
        if (grounded && (angle > stats.minAngleToRotate))
        {
            zRotation = transform.eulerAngles.z;
        }
        else
        {
            zRotation = 0;
        }

        var newRotation = Quaternion.Euler(0, 0, zRotation) * Quaternion.Euler(0, yRotation, 0);

        if (disableSkinRotation)
        {
            skin.root.rotation = Quaternion.identity * Quaternion.Euler(0, yRotation, 0);
        }
        else
        {
            var maxDegree = 850f * Time.deltaTime;
            skin.root.rotation = Quaternion.RotateTowards(skin.root.rotation, newRotation, 10000000f);
        }

        skin.root.position = position;
    }
    private void InitializeSkin()
    {
        direction = 1;
        skin.root.parent = null;
    }

    protected override void OnGroundEnter()
    {
        particles.landSmoke.Play();
    }
    private void ClampToStageBounds()
    {
        var stageManager = StageManager.Instance;

        if (!stageManager || disableCollision) return;
        var nextPosition = position;
			
        if ((nextPosition.x - currentBounds.extents.x - wallExtents) < stageManager.bounds.xMin)
        {
            var safeDistance = stageManager.bounds.xMin + currentBounds.extents.x;
            nextPosition.x = Mathf.Max(nextPosition.x, safeDistance);
            velocity.x = Mathf.Max(velocity.x, 0);
        }
        else if ((nextPosition.x + currentBounds.extents.x + wallExtents) > stageManager.bounds.xMax)
        {
            var safeDistance = stageManager.bounds.xMax - currentBounds.extents.x;
            nextPosition.x = Mathf.Min(nextPosition.x, safeDistance);
            velocity.x = Mathf.Min(velocity.x, 0);
        }

        if ((nextPosition.y - height * 0.5f) < stageManager.bounds.yMin)
        {
            var safeDistance = stageManager.bounds.yMin - height * 0.5f;
            nextPosition.y = Mathf.Max(nextPosition.y, safeDistance);
            ApplyDeath();
        }

        position = nextPosition;
    }
}
