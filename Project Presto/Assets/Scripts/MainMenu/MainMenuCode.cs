using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuCode : MonoBehaviour
{
    public enum MainMenu { Quit,StartGame,Options}
    public GameObject QuitOption;
    public GameObject StartGame;
    public GameObject Options;
    float timeLeft;
    Color targetColor;
 
    void Update()
    {
        if (timeLeft <= Time.deltaTime)
        {
            // transition complete
            // assign the target color
            GetComponent<Image>().color = targetColor;
 
            // start a new transition
            targetColor = new Color(Random.value, Random.value, Random.value,100.0f/256.0f);
            timeLeft = 1.0f;
        }
        else
        {
            // transition in progress
            // calculate interpolated color
            GetComponent<Image>().color = Color.Lerp(GetComponent<Image>().color, targetColor, Time.deltaTime / timeLeft);
 
            // update the timer
            timeLeft -= Time.deltaTime;
        }
    }
    public void ExecuteMenuOption(MainMenu myarg)
    {
        switch (myarg)
        {
            case MainMenu.StartGame:
                SceneManager.LoadScene(1);
                break;
            case MainMenu.Quit:
                Application.Quit();
                break;
            case MainMenu.Options:
                break;
        }
    }
}
