using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathZone : MonoBehaviour {
    void OnTriggerStay(Collider other) {
        var characters = other.gameObject.GetComponentsInParent<Character>();
        if (characters.Length == 0) return;
        var character = characters[0];
        if (character.InStateGroup("death")) return;
        character.stateCurrent = "dying";
    }
}
