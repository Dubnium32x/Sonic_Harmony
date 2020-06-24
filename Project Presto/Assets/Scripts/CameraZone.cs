using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class CameraZone : MonoBehaviour {
    public Vector2 cameraMax;
    public Vector2 cameraMin;

    HashSet<Character> charactersHit = new HashSet<Character>();
    public UnityEvent initialHitEvent;

    Level level;
    public Vector2 positionMax;
    public Vector2 positionMin;
    public bool unloadUnpopulatedLevels = false;

    void Start() {
        level = GetComponentInParent<Level>();

        if (cameraMin == Vector2.zero) cameraMin = Vector2.one * -Mathf.Infinity;
        if (cameraMax == Vector2.zero) cameraMax = Vector2.one * Mathf.Infinity;
    }

    void OnTriggerEnter(Collider other) {
        var characters = other.gameObject.GetComponentsInParent<Character>();
        if (characters.Length == 0) return;

        Set(characters[0]);

        if (charactersHit.Count < LevelManager.current.characters.Count) return;
        if (!unloadUnpopulatedLevels) return;
        
        foreach (Level level in FindObjectsOfType<Level>())
            level.Unload();
    }

    public void Set(Character character) {
        if (character.currentLevel != level) return;
        if (character.characterCamera == null) return;
        var characterCamera = character.characterCamera;
        characterCamera.minPosition = cameraMin;
        characterCamera.maxPosition = cameraMax;
        if (positionMin != Vector2.zero) character.positionMin = positionMin;
        if (positionMax != Vector2.zero) character.positionMax = positionMax;

        if (charactersHit.Count == 0)
            initialHitEvent.Invoke();

        charactersHit.Add(character);
        character.characterCamera.cameraZone = this;
    }
}