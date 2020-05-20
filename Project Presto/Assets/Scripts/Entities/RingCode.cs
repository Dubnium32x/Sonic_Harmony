using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RingCode : MonoBehaviour
{
    // Start is called before the first frame update
    public AudioSource RingDing;
    private Rigidbody NewRB3d;
    private BoxCollider NewBox;
    private bool RingSpecialSpawn = false;
    void Start()
    {
        RingDing = gameObject.GetComponentInChildren<AudioSource>();
    }

    // Update is called once per frame
    public void FixedUpdate()
    {
        if (!RingSpecialSpawn) return;
        if (NewBox == null || NewRB3d == null) return;
        if(Mathf.Abs(NewRB3d.velocity.x) <= 0 && Mathf.Abs(NewRB3d.velocity.y) <= 0)
        {
            NewRB3d.Sleep();
            Destroy(NewRB3d);
            Destroy(NewBox);
            RingSpecialSpawn = false;
        }
    }
    public void OnTriggerEnter(Collider other)
    {
        if(other == null) return;
        if(other.attachedRigidbody?.gameObject?.name == null) return;
        if(other.attachedRigidbody.gameObject.name != "Player_sonic") return;
        var setup = other.GetComponent<CharControlMotor>();
        setup.RingGot();
        gameObject.SetActive(false);
        Destroy(gameObject,5);
    }

    public void RingHurtSpawn(Vector3 targetDir)
    {
        NewBox = gameObject.AddComponent<BoxCollider>();
        NewRB3d = gameObject.AddComponent<Rigidbody>();
        NewRB3d.freezeRotation = true;
        NewRB3d.velocity = targetDir;
        RingSpecialSpawn = true;
        Debug.Break();
    }
}
