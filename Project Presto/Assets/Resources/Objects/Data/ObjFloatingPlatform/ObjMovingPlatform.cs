using UnityEngine;

public class ObjMovingPlatform : MonoBehaviour {
    CharacterGroundedDetector groundedDetector;

    Vector3 positionPrev;

    void Awake() {
        groundedDetector = GetComponent<CharacterGroundedDetector>();
        positionPrev = transform.position;
    }

    void Update() {
        foreach (var character in groundedDetector.characters)
        {
            character.position += (transform.position - positionPrev);
        }

        positionPrev = transform.position;
    }
}