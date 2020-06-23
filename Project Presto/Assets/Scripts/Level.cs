using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Level : MonoBehaviour {
    public AudioClip musicIntro;
    public AudioClip musicLoop;
    public CameraZone cameraZoneStart;

    public Vector3 spawnPosition { get {
        var spawnLocation = transform.Find("Spawn Position");
        if (spawnLocation == null) return Vector3.zero;
        return spawnLocation.position;
    }}

    public int act = 0;
    public string zone = "Unknown";

    void Awake() {
        var levelScene = SceneManager.GetSceneByName("Level");
        if (!levelScene.isLoaded)
            SceneManager.LoadScene("Scenes/Level", LoadSceneMode.Additive);
    }

    void Update()
    {
        foreach (var character in LevelManager.current.characters.Where(character => character.currentLevel == this))
        {
            DLEUpdateCharacter(character);
        }
    }

    public void Unload() {
        if (LevelManager.current.characters.Any(character => character.currentLevel == this))
        {
            return;
        }
        SceneManager.UnloadSceneAsync(gameObject.scene);
        Resources.UnloadUnusedAssets();
    }

    public void ReloadFadeOut(Character character = null) {
        if (LevelManager.current.characters.Count == 1) Time.timeScale = 0;

        var screenFade = Instantiate(
            Constants.Get<GameObject>("prefabScreenFadeOut"),
            Vector3.zero,
            Quaternion.identity
        ).GetComponent<ScreenFade>();
    
        if (character != null)
            screenFade.canvas.worldCamera = character.characterCamera.camera;

        MusicManager.current.FadeOut();
        screenFade.onComplete = () => {
            if (LevelManager.current.characters.Count == 1)
            {
                Reload();
            }
            else {
                if (character == null) return;
                ObjTitleCard.Make(character);
                character.Respawn();
            }
        };
    }

    public void Reload() {
        StartCoroutine(Utils.LoadLevelAsync(
            gameObject.scene.path,
            (Level nextLevel) => {
                MusicManager.current.Clear();
                LevelManager.current.ReloadDisposablesScene();
                foreach (var character in LevelManager.current.characters.Where(character => character.currentLevel == this))
                {
                    character.currentLevel = nextLevel;
                    ObjTitleCard.Make(character);
                    character.Respawn();
                }
                SceneManager.UnloadSceneAsync(gameObject.scene);
            },
            true
        ));
    }

    public virtual void DLEUpdateCharacter(Character character) { }
}
