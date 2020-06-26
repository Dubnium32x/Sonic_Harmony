using System.Collections;
using UnityEngine;


[AddComponentMenu("Freedom Engine/Objects/Enemy/Boss")]
public class Boss : EnemyMotor
{
    private float direction;
    public float groundDistance;
    public float groundRayDistance;
    public float rotateTime;
    public LayerMask solidLayer;

    [Header("Boss Settings")] 
    public float speed;
    private bool turning;
    public float wallRayDistance;
    protected override void OnMotorStart()
    {
        InitializeBoss();
    }

    protected override void OnMotorUpdate(float deltaTime)
    {
        if (turning) return;
        var position = transform.position;
        position += Vector3.right * (direction * speed * deltaTime);

        if (Physics.Raycast(position, Vector3.right * direction, wallRayDistance, solidLayer))
        {
            StartCoroutine(Turn());
        }

        if (Physics.Raycast(position, Vector3.down, out var ground, groundRayDistance, solidLayer))
        {
            position.y = ground.point.y + groundDistance;
        }
        else
        {
            StartCoroutine(Turn());
        }

        //wheel.transform.Rotate(wheelRotationAngle * deltaTime, 0, 0);
        transform.position = position;
    }

    protected override void OnMotorRespawned()
    {
        InitializeBoss();
    }

    protected override void OnMotorRepositioned()
    {
        InitializeBoss();
    }

    private void InitializeBoss()
    {
        turning = false;
        direction = -1;
        health = 8;
    }

    private IEnumerator Turn()
    {
        var elapsedTime = 0f;
        var oldEulerY = transform.eulerAngles.y;

        turning = true;

        while (elapsedTime < rotateTime)
        {
            var alpha = elapsedTime / rotateTime;
            var newEulerY = Mathf.LerpAngle(oldEulerY, 90 * direction - 90, alpha);

            transform.rotation = Quaternion.Euler(0, newEulerY, 0);
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        direction *= -1;
        turning = false;
    }
}