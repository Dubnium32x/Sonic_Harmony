using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SPecial : MonoBehaviour
{
    public int forwardSpeed;
    public int steerAngle;
    public float rotationT = 0.25f;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        var x = Input.GetAxis("Horizontal");
        var y = Input.GetAxis("Vertical");

        if (HitTestWithRoad())
        {
            GetComponent<Rigidbody>().velocity += y * transform.forward * forwardSpeed;

            GetComponent<Rigidbody>().AddTorque(transform.up * x * steerAngle, ForceMode.Acceleration);
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "roadWall")
        {
            //var contact:ContactPoint = collision.contacts[0];
            //this.gameObject.transform.position += contact.normal * 0.2;
        }
    }
    private bool HitTestWithRoad()
    {
        var position = transform.position + transform.TransformDirection(Vector3.up) * 0.2f;
        var direction = transform.TransformDirection(Vector3.down);
        var ray = new Ray(position, direction);
        RaycastHit hit;
        var distance = 2;

        Debug.DrawLine(ray.origin, ray.origin + ray.direction * distance, Color.red);
        var inGround = false;
        if (!Physics.Raycast(ray, out hit, distance)) return inGround;
        //if (hit.collider.tag != "road") return inGround;
        transform.position = hit.point;

        //Debug.DrawLine(ray.origin, hit.point, Color.blue);
        Debug.DrawLine(hit.point, hit.point + hit.normal, Color.green);

        var curUpVector = position - hit.point;
        var hitUpVector = hit.normal;
        Debug.DrawLine(hit.point, hit.point + curUpVector.normalized, Color.white);

        var targetQ = Quaternion.identity;
        //TODO: これから進む先でrayを落とす(sphereのvelocity.normalizeを参照)
        var fPosition = transform.position + transform.TransformDirection(new Vector3(0, 2.0f, 1.0f));
        var fDirection = transform.TransformDirection(Vector3.down);
        var fRay = new Ray(fPosition, fDirection);
        RaycastHit fHit;
        var fDistance = 4;
        Debug.DrawLine(fRay.origin, fRay.origin + fRay.direction * fDistance, Color.cyan);
        if (Physics.Raycast(fRay, out fHit, fDistance))
        {
            //if (fHit.collider.tag == "road")
            //{
                Debug.DrawLine(fHit.point, fHit.point + fHit.normal * fDistance, Color.magenta);
                targetQ.SetLookRotation(fHit.point - transform.position, hitUpVector);
            //}
        }

        gameObject.transform.rotation = Quaternion.Slerp(gameObject.transform.rotation, targetQ, rotationT);
        inGround = true;

        return inGround;
    }
}




