using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class GroundModifications : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform spriteTransform;
    public Quaternion SpriteReal;
    public CharacterCTRL CharScript;
    private Rigidbody rb3d;

    // Constants/Variables for raycasting
    private const float DISTANCE_CHECK =1.0f;

    // Constants/Variables for rotation in air
    private const float ROTATE_SPEED = 85.0f;
    private float prevAngle;

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
        // FallToGroundOrJustFall(); - Not sure why this always sets the rotation to Sonic standing when not fast enough
        IdleCheck();
        //ResetSprite(); - Also not sure why reset to Sonic standing after all other calculations regardless
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
        //var layerMask = 1 << 8;
        RaycastHit hit;
        if (!Physics.Raycast(transform.position, -transform.up, out hit, DISTANCE_CHECK))
        {
            // Slowly rotate back to origin if Sonic is in the air at a rotated angle
            RotateTowards(transform.forward, 0, ROTATE_SPEED * Time.deltaTime);

            return;
        }

        // Also don't collide with itself
        if (hit.collider.gameObject == gameObject)
        {
            // Slowly rotate back to origin if Sonic is in the air at a rotated angle
            RotateTowards(transform.forward, 0, ROTATE_SPEED * Time.deltaTime);

            return;
        }

        // Calculate the angle of Sonic's current rotation to that of the ground
        var AngleToGround = Vector3.SignedAngle(-transform.up, -hit.normal, transform.forward);
        /*var Distance = Vector3.Distance(hit.point, transform.position);
        //Prevent Player From Being Stuck
        if ( Distance >= 1.0f)
        {
            transform.Rotate(transform.forward,5.0f);
        }*/

        // Orient Sonic's rotation to that of the ground
        if (!AngleToGround.Equals(0.0f))
        {
            // Rotate Sonic onto the ground
            transform.Rotate(transform.forward, AngleToGround);

            // Save the last calculated angle
            prevAngle = AngleToGround;
        }
    }

    // Rotates Sonic towards the specified rotation value on the given axis. Rotates by the angle each frame
    void RotateTowards(Vector3 axis, float value, float angle)
    {
        if (transform.rotation.z < value)
            transform.Rotate(axis, angle);
        else if (transform.rotation.z > value)
            transform.Rotate(axis, -angle);
    }
}
