using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingCode : MonoBehaviour
{
    // Start is called before the first frame update
    public AudioSource RingDing;
    void Start()
    {
        RingDing = gameObject.GetComponentInChildren<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other == null) return;
        if(other.attachedRigidbody.gameObject.name != "Player_sonic") return;
        var setup = other.GetComponent<CharacterCTRL>();
        setup.RingGot();
        RingDing.Play();
        gameObject.SetActive(false);
        Destroy(gameObject,5);
    }
}
