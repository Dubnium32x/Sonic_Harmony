using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamFollow : MonoBehaviour
{
    Transform thisTrans;
    CharacterCTRL charCtrl;

    public Transform followTarget;
    public float smoothTime = 0.25f, maxSpeed = Mathf.Infinity, vertHoldtime = 0.5f;
    public bool affectedByPause = true, followZ = false;

    public Vector2 vertDistance;
    public float sideDistance, vertHeld;

    Vector2 followSmooth;
    Vector2 currentOffset;

    // Use this for initialization
    void Awake()
    {
        thisTrans = transform;
        UpdateFollowTarget(followTarget);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if(charCtrl.sonicState == CharacterCTRL.SonicState.ChargingPeel || charCtrl.sonicState == CharacterCTRL.SonicState.ChargingSpin)
        {
            vertHeld = 0;
            return;
        }

        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if (input.y != 0 && (charCtrl == null || charCtrl.speedvar.CompareTo(0f) == 0))
        {
            if (Mathf.Sign(input.y) != Mathf.Sign(vertHeld))
                vertHeld = 0;
            if (input.y > 0)
                vertHeld += Time.deltaTime;
            else
                vertHeld -= Time.deltaTime;
        }
        else
            vertHeld = 0;
        Vector2 targetPos = new Vector2(sideDistance * input.x, 0);
        if (Mathf.Abs(vertHeld).CompareTo(vertHoldtime) >= 0)
            targetPos.y = (vertHeld > 0 ? vertDistance.y : vertDistance.x) * input.y;
        else
            targetPos.y = 0;

        currentOffset = Vector2.SmoothDamp(currentOffset, targetPos,
            ref followSmooth, smoothTime, maxSpeed, Time.deltaTime * ((affectedByPause) ? Time.timeScale : 1));

        Vector3 newPos = followTarget.position;
        newPos.x += currentOffset.x;
        newPos.y += currentOffset.y;

        if (followZ)
            newPos.z = followTarget.position.z;
        else
            newPos.z = thisTrans.position.z;

        thisTrans.position = newPos;
    }

    void UpdateFollowTarget(Transform newTarget)
    {
        followTarget = newTarget;
        if (followTarget != null)
            charCtrl = followTarget.GetComponent<CharacterCTRL>();
    }
}
