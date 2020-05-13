using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpringCode : MonoBehaviour
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
        if (other.attachedRigidbody.gameObject.name != "Player_sonic") return;
        var moveDirection = other.gameObject.GetComponent<CharacterCTRL>().moveDirection;
        var mydirection = (moveDirection * -1);
        other.attachedRigidbody.AddForce(mydirection, ForceMode.VelocityChange);
    }
}
