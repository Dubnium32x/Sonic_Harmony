using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class UIScriptBase : MonoBehaviour
{
    // Start is called before the first frame update
    public TextMeshProUGUI scorTextInstance;
    public TextMeshProUGUI ringTextInstance;
    public TextMeshProUGUI timeTextInstance;
    public TextMeshProUGUI livesTextInstance;
    void Start()
    {
        var uis = gameObject.GetComponentsInChildren<TextMeshProUGUI>();
        scorTextInstance = uis.First(UI => UI.gameObject.name == "score_count");
        ringTextInstance = uis.First(UI => UI.gameObject.name == "ring_count");
        timeTextInstance = uis.First(UI => UI.gameObject.name == "time_count");
        livesTextInstance = uis.First(UI => UI.gameObject.name == "lives_count");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

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
