using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class GroundModifications : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform spriteTransform;
    public Quaternion SpriteReal;
    public CharacterCTRL CharScript;
    private Rigidbody rb3d;
    void Start()
    {
        SpriteReal = spriteTransform.rotation;
        rb3d = GetComponent<Rigidbody>();
        CharScript = GetComponent<CharacterCTRL>();
    }

    // Update is called once per frame
    void Update()
    {
        //LoopZChecks();
        OrientToGround();
        FallToGroundOrJustFall();
        IdleCheck();
        ResetSprite();
    }

    void ResetSprite()
    {
        spriteTransform.rotation = SpriteReal;
    }
    void IdleCheck()
    {
        if (spriteTransform.right == transform.right)
        {
            CharScript.CanIdle = true;
        }
        else
        {
            CharScript.CanIdle = false;
        }
    }
    void FallToGroundOrJustFall()
    {
        var speed = rb3d.velocity.magnitude;
        if (speed <= 4.0f)
        {
            transform.rotation = SpriteReal;
        } 
    }
    void OrientToGround()
    {
        var layerMask = 1 << 8;
        RaycastHit hit;
        if (!Physics.Raycast(transform.position, -transform.up, out hit, 10.0f,
            layerMask))
        {
            transform.rotation = SpriteReal;
        }

        var AngleToGround = Vector3.SignedAngle(-transform.up, -hit.normal,transform.forward);
        var Distance = Vector3.Distance(hit.point, transform.position);
        //Prevent Player From Being Stuck
        if ( Distance >= 1.0f)
        {
            transform.Rotate(transform.forward,5.0f);
        }
        if (!AngleToGround.Equals(0.0f))
        {
            transform.Rotate(transform.forward, AngleToGround);
        }
    }
}
