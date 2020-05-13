using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonitorCode : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnTriggerEnter(Collider other)
    {
        if (other == null) return;
        if (other.attachedRigidbody?.gameObject?.name == null) return;
        if (other.attachedRigidbody.gameObject.name != "Player_sonic") return;
        var setup = other.GetComponent<CharacterCTRL>();
        System.Enum.TryParse<CharacterCTRL.MonitorSpecial>(this.name, true, out var myEnum);

        setup.ActivateSpecial(myEnum);
    }
}
