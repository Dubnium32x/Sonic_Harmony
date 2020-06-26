using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjFloatingPlatform : MonoBehaviour {
    public enum PlatformType {
        stationary,
        falling,
        moving,
        trigger
    }

    bool falling = false;
    float fallSpeed = 0;
    float fallTimer;

    public float fallWaitTime = 0.5F;
    public float gravity = -0.007292F;
    CharacterGroundedDetector groundedDetector;
    Transform moveDestinationLocation;

    float moveTime = 0F;
    public float moveTimeMax = 5F;

    private const float nudgeDistance = -3F / 32F;
    float nudgeTime = 0F;
    private const float nudgeTimeMax = 0.5F;
    Vector3 offsetMove;

    // ======================================

    Vector3 offsetNudge;
    Vector3 offsetOriginal;
    public GameObject platformColliderObj;
    Transform platformTransform;
    bool touchedEver = false;
    public PlatformType type = PlatformType.falling;
    bool touched => groundedDetector.characters.Count > 0;

    // Vector3 positionPrev;
    Vector3 position => offsetNudge + offsetOriginal + offsetMove;

    Vector3 moveDestination { get {
        return moveDestinationLocation.position;
    } }

    void Awake() {
        groundedDetector = platformColliderObj.GetComponent<CharacterGroundedDetector>();
        moveDestinationLocation = transform.Find("Move Destination");
        platformTransform = transform.Find("Platform");
    }

    void Start() {
        offsetOriginal = platformTransform.position;
        offsetNudge = Vector3.zero;
        offsetMove = Vector3.zero;
    }

    void UpdateNudge() {
        if (groundedDetector.characters.Count == 0)
            nudgeTime -= Utils.cappedDeltaTime;
        else
            nudgeTime += Utils.cappedDeltaTime;

        nudgeTime = Mathf.Max(0, Mathf.Min(nudgeTimeMax, nudgeTime));
        // offsetNudge.y = EasingFunction.EaseOutSine(
        //     0,
        //     nudgeDistance,
        //     nudgeTime / nudgeTimeMax
        // );
    }

    void UpdateType() {
        switch(type) {
            case PlatformType.falling:
                if (falling) {
                    fallSpeed += gravity;
                    offsetMove.y += fallSpeed;
                }

                if (touchedEver) {
                    fallTimer -= Utils.cappedDeltaTime;
                    falling = fallTimer <= 0; 
                } else if (touched) {
                    touchedEver = true;
                    fallTimer = fallWaitTime;
                }
                break;
            // case PlatformType.moving:
            //     moveTime += Utils.cappedDeltaTime;
            //     moveTime %= moveTimeMax;
            //     var movePercentage = 1 - Mathf.Abs(((moveTime / moveTimeMax) - 0.5F) * 2);
            //     
            //     offsetMove.x = EasingFunction.EaseInOutSine(
            //         0,
            //         moveDestination.x - offsetOriginal.x,
            //         movePercentage
            //     );
            //     offsetMove.y = EasingFunction.EaseInOutSine(
            //         0,
            //         moveDestination.y - offsetOriginal.y,
            //         movePercentage
            //     );
            //     offsetMove.z = EasingFunction.EaseInOutSine(
            //         0,
            //         moveDestination.z - offsetOriginal.z,
            //         movePercentage
            //     );
            //     break;
        }
    }

    void Update() {
        UpdateType();
        UpdateNudge();
        platformTransform.position = position;
    }
}
