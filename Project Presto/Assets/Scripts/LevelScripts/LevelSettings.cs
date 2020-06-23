using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSettings : MonoBehaviour
{
    public GameObject UICanvas;

    public GameObject Player;
    public Text TimerText;
    public float LevelTimeSeconds;
    public int Rings = 0;
    public int Score = 0;
    public int Lives = 3;
    public GameObject RingPrefab;
    public Scene PreviousScene;
    public Vector3 PreviousEscapePoint;
    public bool LevelHasLoaded = false;
    // Start is called before the first frame update
    void Start()
    {
        //globalvariable For Canvas
        UICanvas = gameObject.GetComponentsInChildren<Transform>(true).First(myobject => myobject.gameObject.name == "Main Camera")
            .GetComponentsInChildren<Transform>(true).First(myobject => myobject.gameObject.name == "Canvas").gameObject;
        //globalvariable ForPlayerObject
        Player = gameObject.GetComponentsInChildren<Transform>(true).First(myplayer => myplayer.gameObject.name == "Player").gameObject;
        //globalvariable For Timer
        TimerText = UICanvas.GetComponentsInChildren<Text>(true)
            .First(Timerbits => Timerbits.gameObject.name == "time_count");
    }

    // Update is called once per frame
    void Update()
    {
        if (!LevelHasLoaded) return;
        TimeProcesing();
        KeepDataInSync();
    }

    void TimeProcesing()
    {
        LevelTimeSeconds += Time.deltaTime;
        var minutes = LevelTimeSeconds / 60;
        var seconds = LevelTimeSeconds % 60;
        var TimeText = $"{minutes:00}:{seconds:00}";
        TimerText.text = TimeText;
    }

    void KeepDataInSync()
    {
        UICanvas.BroadcastMessage("ScoreUpdate", Score);
        UICanvas.BroadcastMessage("RingUpdate", Rings);
        UICanvas.BroadcastMessage("LivesUpdate", Lives);
    }

    public void RingAdd()
    {
        Rings += 1;
    }

    public void GotHurtL(bool fatal)
    {
        if (fatal)
        {
            NoRings();
        }
        else
        {
            if (Rings == 0)
            {
                NoRings();
            }
            else
            {
                //DropRings();
                Infiniframes();
            }
        }
    }

    void Infiniframes()
    {
        //Putinvinble frames here
    }

    public void NoRings()
    {
        if (Lives > 0)
        {
            Lives -= 1;
            Infiniframes();
        }
        else
        {
            GameOver();
        }
    }
    void GameOver()
    {
        SceneManager.LoadScene("GameOver");
    }

    public void AddLife()
    {
        Lives += 1;
    }
}
