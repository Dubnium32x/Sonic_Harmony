using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoopEnter : MonoBehaviour
{
    public LoopEnter exitPoint;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerExit(Collider other)
    {
        if (other == null) return;
        if (other.attachedRigidbody?.gameObject?.name == null) return;
        if (other.attachedRigidbody.gameObject.name != "Player_sonic") return;
        var setup = other.GetComponent<CharControlMotor>();
        //setup.SetLoopExitZ(exitPoint.transform.position);
    }
}
