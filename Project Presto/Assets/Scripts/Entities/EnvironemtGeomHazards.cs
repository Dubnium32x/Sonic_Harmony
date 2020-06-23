using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironemtGeomHazards : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.attachedRigidbody.gameObject.name != "Player_sonic") return;
        var mycontrol = other.GetComponent<CharControlMotor>();
        //mycontrol.GotHurt(false);
    }
}
