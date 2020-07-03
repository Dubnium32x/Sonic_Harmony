using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamFollow : MonoBehaviour
{
    public bool affectedByPause = true, followZ = false;
    private bool returningFromLook = false;
    CharControlMotor charCtrl;
    Vector3 currentOffset;

    Vector2 followSmooth;

    [SerializeField] private Vector2 constOffset;

    public Transform followTarget;
    public float sideDistance, vertHeld;
    public float vertLooksmoothTime = 0.1f, maxSpeed = Mathf.Infinity, vertHoldtime = 0.5f;
    Transform thisTrans;

    public Vector2 vertDistance;
    
    [SerializeField] private Vector2 maxZoomOffset;

    private float xSmooth, ySmooth, zSmooth;
    [SerializeField] private float zoomSmoothTime = 0.35f,
        zMin, zMaxIncrease,
        zoomSpeedMin, zoomSpeedMax;

    // Use this for initialization
    void Awake()
    {
        thisTrans = transform;
        UpdateFollowTarget(followTarget);
        currentOffset = new Vector3(0, 0, zMin);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        float scaledDeltaTime = Time.deltaTime * ((affectedByPause) ? Time.timeScale : 1);
        var state = charCtrl.state.stateId;
        // Stop following Sonic when he dies
        if (state == CharControlMotor.SonicState.Dead)
        {
            return;
        }
        // Set the camera to Sonic's location when charging spin or peel
        else if (state == CharControlMotor.SonicState.ChargingPeel ||
                 state == CharControlMotor.SonicState.ChargingSpin ||
                 state == CharControlMotor.SonicState.Peel ||
                 state == CharControlMotor.SonicState.Spindash)
        {
            vertHeld = 0;
        }

        var input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if (input.y != 0 && (charCtrl == null || charCtrl.velocity.sqrMagnitude.CompareTo(0f) == 0))
        {
            if (Mathf.Sign(input.y) != Mathf.Sign(vertHeld))
                vertHeld = 0;
            if (input.y > 0)
                vertHeld += Time.deltaTime;
            else
                vertHeld -= Time.deltaTime;
        }
        else
        {
            vertHeld = 0;
        }

        float xZoomAmount = maxZoomOffset.x * Mathf.Clamp01((Mathf.Abs(charCtrl.velocity.x) - zoomSpeedMin) / zoomSpeedMax) * Mathf.Sign(charCtrl.velocity.x),
            yZoomAmount = maxZoomOffset.y * Mathf.Clamp01((Mathf.Abs(charCtrl.velocity.y) - zoomSpeedMin) / zoomSpeedMax) * Mathf.Sign(charCtrl.velocity.y),
            zZoomAmount = zMaxIncrease * Mathf.Clamp01((Mathf.Abs(charCtrl.velocity.magnitude) - zoomSpeedMin) / zoomSpeedMax) + zMin;

        currentOffset.x = Mathf.SmoothDamp(currentOffset.x, xZoomAmount, ref xSmooth, zoomSmoothTime, Mathf.Infinity, Time.deltaTime);

        if (charCtrl.velocity.sqrMagnitude > 0)
            returningFromLook = false;

        //var targetPos = new Vector3(sideDistance * input.x, 0, 0);
        if (Mathf.Abs(vertHeld).CompareTo(vertHoldtime) >= 0)
        {
            currentOffset.y = Mathf.SmoothDamp(currentOffset.y, (vertHeld > 0 ? vertDistance.y : -vertDistance.x), ref ySmooth, vertLooksmoothTime, Mathf.Infinity, Time.deltaTime);
            returningFromLook = true;
        }
        else if(returningFromLook)
        {
            currentOffset.y = Mathf.SmoothDamp(currentOffset.y, yZoomAmount, ref ySmooth, vertLooksmoothTime, Mathf.Infinity, Time.deltaTime);
        }
        else
        {
            currentOffset.y = Mathf.SmoothDamp(currentOffset.y, yZoomAmount, ref ySmooth, zoomSmoothTime, Mathf.Infinity, Time.deltaTime);
        }

        currentOffset.z = Mathf.SmoothDamp(currentOffset.z, zZoomAmount, ref zSmooth, zoomSmoothTime, Mathf.Infinity, Time.deltaTime);

        /*currentOffset = Vector2.SmoothDamp(currentOffset, targetPos,
            ref followSmooth, vertLooksmoothTime, maxSpeed, scaledDeltaTime);*/

        var newPos = followTarget.position;
        newPos.x += currentOffset.x + constOffset.x;
        newPos.y += currentOffset.y + constOffset.y;
        newPos.z += currentOffset.z;

        /*if (followZ)
            newPos.z = followTarget.position.z;
        else
            newPos.z = thisTrans.position.z;*/

        thisTrans.position = newPos;
    }

    void UpdateFollowTarget(Transform newTarget)
    {
        followTarget = newTarget;
        if (followTarget != null)
            charCtrl = followTarget.GetComponent<CharControlMotor>();
    }
}
