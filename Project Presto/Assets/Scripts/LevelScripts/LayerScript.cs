using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerScript : MonoBehaviour
{
    public float ZvalueToSet;

    private void OnTriggerEnter(Collider other)
    {
        if (other == null) return;
        if (other.attachedRigidbody.gameObject.name != "Player_sonic") return;
        var setup = other.GetComponent<CharControlMotor>();
        //setup.SetZlayer(ZvalueToSet);
    }
}
