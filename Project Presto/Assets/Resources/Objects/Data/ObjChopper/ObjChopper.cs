using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjChopper : MonoBehaviour
{
    public float gravity = -0.17578125F;
    public float jumpSpeed = 13.125F;
    Vector3 positionOrig;
    new Rigidbody rigidbody => GetComponent<Rigidbody>();


    void Start() { positionOrig = transform.position; }

    void Update() {
        if (transform.position.y <= positionOrig.y) {
            rigidbody.velocity = new Vector3(0, jumpSpeed * Utils.physicsScale);
            transform.position = positionOrig;
        }

        rigidbody.velocity += new Vector3(0, gravity * Utils.physicsScale * Utils.deltaTimeScale);
    }
}
