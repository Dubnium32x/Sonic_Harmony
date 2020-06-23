using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneControl : MonoBehaviour {
    public void StartScene(string scenePath) {
        var screenFade = Instantiate(
            Constants.Get<GameObject>("prefabScreenFadeOut"),
            Vector3.zero,
            Quaternion.identity
        ).GetComponent<ScreenFade>();
        screenFade.stopTime = true;
        screenFade.onComplete = () => {
            var asyncLoad = SceneManager.LoadSceneAsync(
                scenePath,
                LoadSceneMode.Single
            );
            asyncLoad.allowSceneActivation = true;
        };
        enabled = false;
    }
}