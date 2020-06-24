using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuCode : MonoBehaviour
{
    public enum MainMenu { Quit,StartGame,Options}
    public GameObject QuitOption;
    public GameObject StartGame;
    public GameObject Options;

    public void ExecuteMenuOption(MainMenu myarg)
    {
        switch (myarg)
        {
            case MainMenu.StartGame:
                break;
            case MainMenu.Quit:
                break;
            case MainMenu.Options:
                break;
        }
    }
}
