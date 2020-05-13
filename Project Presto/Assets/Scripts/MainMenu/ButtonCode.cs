using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonCode : MonoBehaviour
{
    public MainMenuCode mainMenu;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnMouseUp()
    {
        System.Enum.TryParse<MainMenuCode.MainMenu>(this.name, true, out var myEnum);
        mainMenu.ExecuteMenuOption(myEnum);
    }
}
