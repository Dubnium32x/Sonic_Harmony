using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ProBuilder2.Common;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIScriptBase : MonoBehaviour
{
    // Start is called before the first frame update
    public Text scorTextInstance;
    public Text ringTextInstance;
    public Text timeTextInstance;
    public Text livesTextInstance;
    void Start()
    {
        var uis = gameObject.GetComponentsInChildren<Text>();
        scorTextInstance = uis.First(UI => UI.gameObject.name == "score_count");
        ringTextInstance = uis.First(UI => UI.gameObject.name == "ring_count");
        timeTextInstance = uis.First(UI => UI.gameObject.name == "time_count");
        livesTextInstance = uis.First(UI => UI.gameObject.name == "lives_count");
    }

    // Update is called once per frame

    public void ScoreUpdate(int score)
    {
        scorTextInstance.text = score.ToString();
    }

    public void RingUpdate(int rings)
    {
        ringTextInstance.text = rings.ToString();
    }

    public void LivesUpdate(int lives)
    {
        livesTextInstance.text = lives.ToString();
    }

}
