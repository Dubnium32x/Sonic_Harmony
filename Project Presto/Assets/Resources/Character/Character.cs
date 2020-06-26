using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
using System;
using System.Linq;

public class Character : GameBehaviour
{
    //This is where the balence part starts
    public enum BalanceState
    {
        None,
        Left, // platform is to the left
        Right // platform is to the right
    }

    // ========================================================================

    public static LayerMask? _solidRaycastMask = null;

    // ========================================================================

    CharacterGroundedDetector _groundedDetectorCurrent;

    // [SyncVar]
    float _groundSpeed = 0;

    int _lives = 3;

    // ========================================================================

    Transform _modeGroupCurrent;

    int _ringLivesMax = 0;
    int _rings;

    int _score = 0;
    string _stateCurrent = "ground";

    Dictionary<string, List<string>> _stateGroups = new Dictionary<string, List<string>>();
    [HideInInspector] public Collider airModeCollider;
    [HideInInspector] public Transform airModeGroup;

    // ========================================================================

    [HideInInspector] public AudioSource audioSource;

    // [HideInInspector, SyncVar]
    [HideInInspector] public BalanceState balanceState = BalanceState.None;

    public GameObject cameraPrefab;
    // ========================================================================
    // ========================================================================

    public List<CharacterCapability> capabilities = new List<CharacterCapability>();

    // ========================================================================

    [HideInInspector] public CharacterCamera characterCamera;

    public int checkpointId = 0;
    [HideInInspector] public Collider colliderCurrent;
    public bool controlLockManual = false;

    // ========================================================================

    public Level currentLevel;

    public int destroyEnemyChain = 0;

    // ========================================================================

    public List<CharacterEffect> effects = new List<CharacterEffect>();

    // ========================================================================

    // [SyncVar]
    public bool facingRight = true;

    // [SyncVar]
    public bool flipX = false;
    // ========================================================================

    [HideInInspector] public Transform groundModeGroup;

    // ========================================================================

    // [HideInInspector, SyncVar]
    [HideInInspector] public float groundSpeedPrev = 0;

    // ========================================================================

    [HideInInspector] public float horizontalInputLockTimer = 0;

    [HideInInspector] HUD hud;
    public GameObject hudPrefab;

    public InputCustom input;

    // ========================================================================

    public bool isGhost = false;
    public bool isLocalPlayer = true;

    public float opacity = 1;
    public int playerId = -1;

    public Vector2 positionMax = new Vector2(
        Mathf.Infinity,
        Mathf.Infinity
    );

    // ========================================================================

    public Vector2 positionMin = new Vector2(
        -Mathf.Infinity,
        -Mathf.Infinity
    );

    public RespawnData respawnData = new RespawnData();

    // ========================================================================

    [HideInInspector] public new Rigidbody rigidbody;
    [HideInInspector] public Collider rollingAirModeCollider;
    [HideInInspector] public Transform rollingAirModeGroup;
    [HideInInspector] public Transform rollingModeGroup;

    // ========================================================================

    public ObjShield shield;

    // ========================================================================

    // [SyncVar]
    public float sizeScale = 1F;

    // ========================================================================

    [HideInInspector] public SpriteRenderer sprite;
    [HideInInspector] public Animator spriteAnimator;

    // [HideInInspector, SyncVar]
    [HideInInspector] public float spriteAnimatorSpeed;

    // [HideInInspector, SyncVar]
    [HideInInspector] public string spriteAnimatorState;

    [HideInInspector] string spriteAnimatorStatePrev;
    [HideInInspector] public Transform spriteContainer;
    public GameObject spriteContainerPrefab;

    [HideInInspector] public string statePrev = "ground";
    public CharacterStats stats = new CharacterStats();
    public float timer = 0;
    public bool timerPause = false;

    // [HideInInspector, SyncVar]
    [HideInInspector] public Vector3 velocityPrev;

    public string stateCurrent
    {
        get { return _stateCurrent; }
        set => StateChange(value);
    }

    // ========================================================================

    public Vector3 position
    {
        get { return transform.position; }
        set { transform.position = value; }
    }


    public Vector3 eulerAngles
    {
        get { return transform.eulerAngles; }
        set { transform.eulerAngles = value; }
    }

    public float forwardAngle
    {
        get => eulerAngles.z;
        set
        {
            var angle = eulerAngles;
            angle.z = value;
            eulerAngles = angle;
        }
    }

    public bool movingUphill => Math.Abs(Mathf.Sign(groundSpeed) - Mathf.Sign(Mathf.Sin(forwardAngle * Mathf.Deg2Rad))) < float.Epsilon;

    public Vector3 velocity
    {
        get { return rigidbody.velocity; }
        set => rigidbody.velocity = value;
    }

    public float groundSpeed
    {
        get { return _groundSpeed; }
        set
        {
            _groundSpeed = value;
            groundSpeedRigidbody = _groundSpeed;
        }
    }

    // 3D-Ready: NO
    public float groundSpeedRigidbody
    {
        get
        {
            return (
                (velocity.x * Mathf.Cos(forwardAngle * Mathf.Deg2Rad)) +
                (velocity.y * Mathf.Sin(forwardAngle * Mathf.Deg2Rad))
            );
        }
        set
        {
            velocity = new Vector3(
                Mathf.Cos(forwardAngle * Mathf.Deg2Rad),
                Mathf.Sin(forwardAngle * Mathf.Deg2Rad),
                velocity.z
            ) * value;
        }
    }

    public static LayerMask solidRaycastMask
    {
        get
        {
            if (_solidRaycastMask != null) return (LayerMask) _solidRaycastMask;
            _solidRaycastMask = LayerMask.GetMask(
                "Ignore Raycast",
                "Player - Ignore Top Solid and Raycast",
                "Player - Ignore Top Solid",
                "Player - Rolling",
                "Player - Rolling and Ignore Top Solid",
                "Player - Rolling and Ignore Top Solid",
                "Object - Player Only and Ignore Raycast",
                "Object - Ring"
            );
            return (LayerMask) _solidRaycastMask;
        }
    }

    public CharacterGroundedDetector groundedDetectorCurrent
    {
        get => _groundedDetectorCurrent;
        set
        {
            if (value == _groundedDetectorCurrent) return;
            if (_groundedDetectorCurrent != null) _groundedDetectorCurrent.Leave(this);
            _groundedDetectorCurrent = value;
            if (_groundedDetectorCurrent != null) _groundedDetectorCurrent.Enter(this);
        }
    }

    public float physicsScale => sizeScale * Utils.physicsScale;

    public int score
    {
        get { return _score; }
        set
        {
            if (Mathf.Floor(value / 50000F) > Mathf.Floor(_score / 50000F))
                lives++;

            _score = value;
        }
    }

    public int rings
    {
        get { return _rings; }
        set
        {
            _rings = value;
            var livesPrev = lives;
            lives += Mathf.Max(0, (int) Mathf.Floor(_rings / 100F) - _ringLivesMax);
            _ringLivesMax = Mathf.Max(_ringLivesMax, (int) Mathf.Floor(_rings / 100F));
        }
    }

    public int lives
    {
        get { return _lives; }
        set
        {
            if (value > _lives)
            {
                MusicManager.current.Add(new MusicManager.MusicStackEntry
                {
                    introPath = "Music/1-Up",
                    disableSfx = true,
                    fadeInAfter = true,
                    priority = 10,
                    ignoreClear = true
                });
            }

            _lives = value;
        }
    }

    public bool controlLock =>
    (
        controlLockManual ||
        Math.Abs(Time.timeScale) < float.Epsilon ||
        InStateGroup("noControl") ||
        !isLocalPlayer
    );

    public Transform modeGroupCurrent
    {
        get { return _modeGroupCurrent; }
        set
        {
            if (_modeGroupCurrent == value) return;

            if (_modeGroupCurrent != null)
                _modeGroupCurrent.gameObject.SetActive(false);

            _modeGroupCurrent = value;
            colliderCurrent = null;

            if (_modeGroupCurrent == null) return;
            _modeGroupCurrent.gameObject.SetActive(true);
            colliderCurrent = _modeGroupCurrent.Find("Collider").GetComponent<Collider>();
        }
    }

    // ========================================================================

    public bool isInvulnerable => (
        InStateGroup("ignore") ||
        HasEffect("invulnerable") ||
        HasEffect("invincible")
    );

    public bool isHarmful => (
        InStateGroup("harmful") ||
        HasEffect("invincible")
    );

    // ========================================================================
    public bool pressingLeft => input.GetAxisNegative("Horizontal");
    public bool pressingRight => input.GetAxisPositive("Horizontal");

    public CharacterCapability GetCapability(string capabilityName)
    {
        return capabilities.FirstOrDefault(capability => capability.name == capabilityName);
    }

    public CharacterCapability TryGetCapability(string capabilityName, Action<CharacterCapability> callback)
    {
        var capability = GetCapability(capabilityName);
        if (capability != null) callback(capability);
        return capability;
    }

    // ========================================================================


    void StateChange(string newState)
    {
        var stateCurrentCheck = stateCurrent;
        if (_stateCurrent == newState) return;
        foreach (var capability in capabilities)
            capability.StateDeinit(_stateCurrent, newState);

        if (stateCurrentCheck != stateCurrent) return; // Allows changing state in Deinit

        statePrev = _stateCurrent;
        _stateCurrent = newState;
        foreach (var capability in capabilities)
            capability.StateInit(_stateCurrent, statePrev);
    }

    public bool InStateGroup(string groupName)
    {
        return InStateGroup(groupName, stateCurrent);
    }

    public bool InStateGroup(string groupName, string stateName)
    {
        if (!_stateGroups.ContainsKey(groupName)) return false;
        return _stateGroups[groupName].Contains(stateName);
    }

    public void AddStateGroup(string groupName, string stateName)
    {
        if (!_stateGroups.ContainsKey(groupName))
        {
            _stateGroups[groupName] = new List<string> {stateName};
        }
        else
        {
            _stateGroups[groupName].Add(stateName);
        }
    }

    public void UpdateEffects(float deltaTime)
    {
        // iterate backwards to prevent index from shifting
        // (effects remove themselves once complete)
        for (var i = effects.Count - 1; i >= 0; i--)
        {
            var effect = effects[i];
            effect.UpdateBase(deltaTime);
        }
    }

    public CharacterEffect GetEffect(string effectName)
    {
        return effects.FirstOrDefault(effect => effectName == effect.name);
    }

    public bool HasEffect(string effectName)
    {
        return GetEffect(effectName) != null;
    }

    public void ClearEffects()
    {
        // iterate backwards to prevent index from shifting
        for (var i = effects.Count - 1; i >= 0; i--)
        {
            var effect = effects[i];
            effect.DestroyBase();
        }
    }

    public void GroundSpeedSync()
    {
        _groundSpeed = groundSpeedRigidbody;
    }

    // ========================================================================

    public RaycastHit GetGroundRaycast()
    {
        return GetSolidRaycast(-transform.up);
    }

    // 3D-Ready: YES
    public RaycastHit GetSolidRaycast(Vector3 direction, float maxDistance = 0.8F)
    {
        Physics.Raycast(
            position, // origin
            direction.normalized, // direction
            out var hit,
            maxDistance * sizeScale, // max distance
            ~solidRaycastMask // layer mask
        );
        return hit;
    }

    bool GetIsGrounded()
    {
        var hit = GetGroundRaycast();
        return GetIsGrounded(hit);
    }

    // 3D-Ready: NO
    bool GetIsGrounded(RaycastHit hit)
    {
        // Potentially avoid recomputing raycast
        if (hit.collider == null) return false;

        var hitAngle = Quaternion.FromToRotation(Vector3.up, hit.normal).eulerAngles.z;
        var angleDiff = Mathf.DeltaAngle(
            hitAngle,
            forwardAngle
        );

        var collisionModifier = hit.transform.GetComponentInParent<CharacterCollisionModifier>();
        if (collisionModifier == null) return angleDiff < 67.5F;
        switch (collisionModifier.type)
        {
            case CharacterCollisionModifier.CollisionModifierType.NoGrounding:
                return false;
            case CharacterCollisionModifier.CollisionModifierType.NoGroundingLRB:
                if (hitAngle > 90 && hitAngle < 270) return false;
                break;
            case CharacterCollisionModifier.CollisionModifierType.NoGroundingLRBHigher:
                if (hitAngle > 45 && hitAngle < 315) return false;
                break;
        }

        return angleDiff < 67.5F;
    }

    //this is where that enum gets used
    // Keeps character locked to ground while in ground state
    // 3D-Ready: No, but pretty close, actually.
    public bool GroundSnap()
    {
        var hit = GetGroundRaycast();
        balanceState = BalanceState.None;

        if (GetIsGrounded(hit))
        {
            transform.eulerAngles = new Vector3(
                0,
                0,
                Quaternion.FromToRotation(Vector3.up, hit.normal).eulerAngles.z
            );

            var newPos = hit.point + (transform.up * (0.5F * sizeScale));
            newPos.z = position.z; // Comment this for 3D movement
            position = newPos;
            groundedDetectorCurrent = hit.transform.GetComponentInChildren<CharacterGroundedDetector>();
            return true;
        }

        // Didn't find the ground from the player center?
        // We might be on a ledge. Better check to the left and right of
        // the character to be sure.
        for (var dir = -1; dir <= 1; dir += 2)
        {
            RaycastHit hitLedge;
            Physics.Raycast(
                position + (transform.right * (dir * 0.375F * sizeScale * sizeScale)), // origin
                -transform.up, // direction
                out hitLedge,
                0.8F * sizeScale, // max distance
                ~solidRaycastMask // layer mask
            );
            if (!GetIsGrounded(hitLedge)) continue;
            balanceState = dir < 0 ? BalanceState.Left : BalanceState.Right;
            groundedDetectorCurrent = hitLedge.transform.GetComponentInChildren<CharacterGroundedDetector>();

            var newPos = (
                hitLedge.point -
                (transform.right * (dir * 0.375F * sizeScale)) +
                (transform.up * (0.5F * sizeScale))
            );
            newPos.x = position.x;
            newPos.z = position.z;
            position = newPos;
            return true;
        }

        if (stateCurrent == "rolling") stateCurrent = "rollingAir";
        else stateCurrent = "air";

        return false;
    }

    public void AnimatorPlay(string stateName, float normalizedTime = float.NegativeInfinity)
    {
        spriteAnimatorStatePrev = spriteAnimatorState;
        spriteAnimator.Play(stateName, -1, normalizedTime);
        spriteAnimatorState = stateName;
    }

    // Gets rotation for sprite
    // 3D-Ready: No
    public Vector3 GetSpriteRotation(float deltaTime)
    {
        if (!GlobalOptions.GetBool("smoothRotation"))
            return (transform.eulerAngles / 45F).Round(0) * 45F;

        var currentRotation = sprite.transform.eulerAngles;
        var shouldRotate = Mathf.Abs(Mathf.DeltaAngle(0, forwardAngle)) > 45;

        Vector3 targetAngle;
        if (shouldRotate)
        {
            targetAngle = transform.eulerAngles;
            if (forwardAngle > 180 && currentRotation.z == 0)
                currentRotation.z = 360;
        }
        else
        {
            targetAngle = currentRotation.z > 180 ? new Vector3(0, 0, 360) : Vector3.zero;
        }

        return Vector3.RotateTowards(
            currentRotation, // current
            targetAngle, // target // TODO: 3D
            10F * deltaTime * 60F, // max rotation
            10F * deltaTime * 60F // magnitude
        );
    }

    // 3D-Ready: YES

    public void RemoveShield()
    {
        if (shield == null) return;
        Destroy(shield.gameObject);
        shield = null;
    }

    public void Respawn()
    {
        SoftRespawn();
        if (checkpointId == 0)
        {
            if (currentLevel.cameraZoneStart != null)
                currentLevel.cameraZoneStart.Set(this);
        }

        if (characterCamera == null) return;
        characterCamera.MinMaxPositionSnap();
        characterCamera.position = transform.position;
    }

    public void SoftRespawn()
    {
        // Should only be used in multiplayer; for full respawns reload scene
        _rings = 0;
        _ringLivesMax = 0;
        effects.Clear();
        position = respawnData.position;
        timer = respawnData.timer;
        stateCurrent = "ground";
        velocity = Vector3.zero;
        _groundSpeed = 0;
        transform.eulerAngles = Vector3.zero;
        facingRight = true;

        timerPause = false;
        TryGetCapability("victory",
            (CharacterCapability capability) => { ((CharacterCapabilityVictory) capability).victoryLock = false; });
    }

    public void LimitPosition()
    {
        var positionNew = Vector2.Min(
            Vector2.Max(
                position,
                positionMin
            ),
            positionMax
        );

        if ((Vector2) position == positionNew) return;
        position = positionNew;
        _groundSpeed = 0;
    }

    // 3D-Ready: NO
    public void Hurt(bool moveLeft = true, bool spikes = false)
    {
        if (isInvulnerable) return;

        if (shield != null)
        {
            SFX.Play(audioSource, "sfxHurt");
            RemoveShield();
        }
        else if (rings == 0)
        {
            if (spikes) SFX.Play(audioSource, "sfxDieSpikes");
            else SFX.Play(audioSource, "sfxDie");
            stateCurrent = "dying";
            return;
        }
        else
        {
            ObjRing.ExplodeRings(transform.position, Mathf.Min(rings, 32));
            rings = 0;
            SFX.Play(audioSource, "sfxHurtRings");
        }

        stateCurrent = "hurt";
        velocity = new Vector3( // TODO: 3D
            2 * (moveLeft ? -1 : 1) * physicsScale,
            4 * physicsScale,
            velocity.z
        );
        position += velocity / 30F; // HACK
    }


    public virtual void Start()
    {
        rollingModeGroup.gameObject.SetActive(false);
        groundModeGroup.gameObject.SetActive(false);
        rollingAirModeGroup.gameObject.SetActive(false);
        airModeGroup.gameObject.SetActive(false);

        foreach (CharacterCapability capability in capabilities)
            capability.StateInit(stateCurrent, "");

        if (isLocalPlayer)
        {
            characterCamera = Instantiate(cameraPrefab).GetComponent<CharacterCamera>();
            characterCamera.character = this;
            characterCamera.UpdateDelta(0);

            ObjTitleCard titleCard = ObjTitleCard.Make(this);

            hud = Instantiate(hudPrefab).GetComponent<HUD>();
            hud.character = this;
            hud.Update();
        }

        respawnData.position = position;
        Respawn();
    }

    public override void UpdateDelta(float deltaTime)
    {
        UpdateEffects(deltaTime);
        stats.physicsScale = physicsScale;

        if (!isLocalPlayer)
        {
            isGhost = true;
            if (spriteAnimatorState != spriteAnimatorStatePrev)
            {
                spriteAnimator.Play(spriteAnimatorState);
                spriteAnimatorStatePrev = spriteAnimatorState;
            }
        }
        else
        {
            groundSpeedPrev = groundSpeed;
            if (!timerPause) timer += deltaTime * Time.timeScale;
            if (!isHarmful) destroyEnemyChain = 0;

            foreach (var capability in capabilities)
            {
                capability.Update(deltaTime);
                input.enabled = !controlLock;
            }

            velocityPrev = velocity;
        }

        spriteAnimator.speed = spriteAnimatorSpeed;
        transform.localScale = new Vector3(sizeScale, sizeScale, sizeScale);
        spriteContainer.localScale = new Vector3( // Hacky
            sizeScale * (flipX ? -1 : 1),
            sizeScale * Mathf.Sign(spriteContainer.localScale.y),
            sizeScale * Mathf.Sign(spriteContainer.localScale.z)
        );
        var colorTemp = sprite.color;
        colorTemp.a = opacity * (isGhost ? 0.5F : 1);
        sprite.color = colorTemp;

        LimitPosition();
    }

    public override void LateUpdateDelta(float deltaTime)
    {
        spriteContainer.transform.position = position;

        if (characterCamera != null)
            characterCamera.UpdateDelta(deltaTime);
    }

    // ========================================================================

    public void OnCollisionEnter(Collision collision)
    {
        foreach (var capability in capabilities)
            capability.OnCollisionEnter(collision);
    }

    public void OnCollisionStay(Collision collision)
    {
        foreach (var capability in capabilities)
            capability.OnCollisionStay(collision);
    }

    public void OnCollisionExit(Collision collision)
    {
        foreach (var capability in capabilities)
            capability.OnCollisionExit(collision);
    }

    public void OnTriggerEnter(Collider other)
    {
        foreach (var capability in capabilities)
            capability.OnTriggerEnter(other);
    }

    public void OnTriggerStay(Collider other)
    {
        foreach (var capability in capabilities)
            capability.OnTriggerStay(other);
    }

    public void OnTriggerExit(Collider other)
    {
        foreach (var capability in capabilities)
            capability.OnTriggerExit(other);
    }

    // ========================================================================

    public override void OnDestroy()
    {
        base.OnDestroy();

        LevelManager.current.characters.Remove(this);
        Destroy(characterCamera.gameObject);
        Destroy(spriteContainer.gameObject);
        Destroy(hud.gameObject);

        // Keep player IDs sequential
        if (playerId < 0) return;
        foreach (var character in LevelManager.current.characters.Where(character => character.playerId > playerId))
        {
            character.playerId--;
        }
    }

    void InitReferences()
    {
        rigidbody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();

        spriteContainer = Instantiate(spriteContainerPrefab).transform;
        sprite = spriteContainer.Find("Sprite").GetComponent<SpriteRenderer>();
        spriteAnimator = sprite.GetComponent<Animator>();

        groundModeGroup = transform.Find("Ground Mode");
        rollingModeGroup = transform.Find("Rolling Mode");
        airModeGroup = transform.Find("Air Mode");
        rollingAirModeGroup = transform.Find("Rolling Air Mode");

        rollingAirModeCollider = rollingAirModeGroup.Find("Collider").GetComponent<Collider>();
        airModeCollider = airModeGroup.Find("Collider").GetComponent<Collider>();
    }

    public override void Awake()
    {
        base.Awake();

        stats.Add(new Dictionary<string, object>()
        {
            ["topSpeedNormal"] = 6F,
            ["topSpeedSpeedUp"] = 12F,
            ["topSpeed"] = (Func<string>) (() =>
            {
                if (HasEffect("speedUp"))
                {
                    return "topSpeedSpeedUp";
                }
                else
                {
                    return "topSpeedNormal";
                }
            }),
            ["terminalSpeed"] = 16.5F
        });

        LevelManager.current.characters.Add(this);
        playerId = LevelManager.current.GetFreePlayerId();
        input = new InputCustom(1);

        InitReferences();

        var levelDefault = FindObjectOfType<Level>();

        if (currentLevel == null)
        {
            currentLevel = levelDefault;
            respawnData.position = levelDefault.spawnPosition;
            Respawn();
        }

        if (GlobalOptions.GetBool("tinyMode"))
            sizeScale = 0.5F;
    }

    public class RespawnData
    {
        public Vector3 position = Vector3.zero;
        public float timer = 0;
    }
}