using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerZone : MonoBehaviour {
    public float zLeft = 0;
    public float zRight = 0;
    public bool groundedOnly = false;

    void OnTriggerStay(Collider other) {
        OnTriggerEnter(other);
    }

    void OnTriggerEnter(Collider other) {
        var characters = other.gameObject.GetComponentsInParent<Character>();
        if (characters.Length == 0) return;
        var character = characters[0];

        var characterPos = character.position;

        if (groundedOnly && !character.InStateGroup("ground"))
            return;

        if (characterPos.x > transform.position.x) characterPos.z = zRight;
        else characterPos.z = zLeft;

        character.position = characterPos;
    }
}
