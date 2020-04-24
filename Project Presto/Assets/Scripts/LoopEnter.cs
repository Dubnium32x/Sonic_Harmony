using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoopEnter : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {

        RaycastHit hit;
        if (Physics.Raycast(transform.position, -transform.right, out hit))
        {
            var thepoint = hit.point;
            //thepoint.y += 10;
            other.attachedRigidbody.AddForce(thepoint*50,ForceMode.Acceleration);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //if (other.transform.position.x - transform.position.x <= 0)
       // {
        //    other.attachedRigidbody.AddForce(new Vector3(50,0,0));
       // }
        //other.gameObject.BroadcastMessage("ActivateLoop");
    }
}
