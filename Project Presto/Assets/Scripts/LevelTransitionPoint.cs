using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelTransitionPoint : MonoBehaviour
{
    public string nextScene;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other == null) return;
        if (other.attachedRigidbody.gameObject.name != "Player_sonic") return;
        var setitings = this.transform.root.GetComponent<LevelSettings>();
        setitings.PreviousEscapePoint = other.transform.position;
        setitings.PreviousScene = gameObject.scene;
        SceneManager.LoadScene(nextScene);
    }
}
