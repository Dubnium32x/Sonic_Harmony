using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadCard : MonoBehaviour
{
    public GameObject Cardbox;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator HandleCardCaper(Scene scene)
    {
        var levelSettings = transform.root.gameObject.GetComponent<LevelSettings>();
        var mycanvas = GetComponentsInChildren<Transform>(true)
            .FirstOrDefault(Result => Result.name == "Canvas").gameObject;
        var cardSprite = Cardbox.GetComponent<SpriteRenderer>();
        var InnerImage = Resources.Load("LevelLoads/" + scene.name) as Texture2D;
        var LevelImage = Sprite.Create(InnerImage, new Rect(0, 0, InnerImage.width, InnerImage.height), new Vector2(0.5f, 0.5f), 100);
        var Player = SceneManager.GetActiveScene()
            .GetRootGameObjects().First(result => result.name.ToLower() == "gamecontroller")
            .GetComponentInChildren<CharControlMotor>();
        cardSprite.sprite = LevelImage;
        Cardbox.SetActive(true);
        yield return new WaitForSeconds(5);
        levelSettings.LevelHasLoaded = true;
        mycanvas.SetActive(true);
        Cardbox.SetActive(false);
        Player.disableInput = false;
        Resources.UnloadAsset(InnerImage);
    }
    // called first
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }    // called when the game is terminated
    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    // called second
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(HandleCardCaper(scene));
    }
}
