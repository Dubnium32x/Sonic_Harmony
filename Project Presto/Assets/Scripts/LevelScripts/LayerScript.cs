using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerScript : MonoBehaviour
{
    public float ZvalueToSet;
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
        if (other == null) return;
        if (other.attachedRigidbody.gameObject.name != "Player_sonic") return;
        var setup = other.GetComponent<CharacterCTRL>();
        setup.SetZlayer(ZvalueToSet);
    }
}
