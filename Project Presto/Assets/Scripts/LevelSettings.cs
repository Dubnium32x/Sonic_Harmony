using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSettings : MonoBehaviour
{
    public GameObject UICanvas;

    public GameObject Player;
    public TextMeshProUGUI TimerText;
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
        Player = gameObject.GetComponentsInChildren<Transform>(true).First(myplayer => myplayer.gameObject.name == "Player_sonic").gameObject;
        //globalvariable For Timer
        TimerText = UICanvas.GetComponentsInChildren<TextMeshProUGUI>(true)
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

    void DropRings()
    {
        var circleRadius = 1;
        var angle = 101.25f; //assuming 0 = right, 90 = up, 180 = left, 270 = down
        int innerRings;
        if (Rings > 32)
        {
            innerRings = 32;
        }
        else
        {
            innerRings = Rings;
        }
        var OpositeValue = false;
        var speed = 4;
        Rings -= innerRings;
        for (var i = 0; i < innerRings; i++)
        {
            var ringpos = Player.transform.position;
            var xCalc = Mathf.Cos(angle * Mathf.Deg2Rad);
            var yCalc = -Mathf.Sin(angle * Mathf.Deg2Rad);
            ringpos.x += speed * xCalc;
            ringpos.y += speed * yCalc;
            if(Physics.Linecast(Player.transform.position, ringpos,out var outHit))
            {
                ringpos = outHit.point;
            }
            var myVeloc = new Vector3(xCalc, yCalc) * speed;

            if (OpositeValue)
            {
                myVeloc.x *= -1;
                ringpos.x *= -1;
                angle += 22.5f;
            }
            OpositeValue = !OpositeValue;
            if (i == 16)
            {
                speed = 2; //we're on the second circle now, so decrease the speed
                angle = 101.25f; //and reset the angle
            }
            var TheRing = Instantiate(RingPrefab, ringpos, Quaternion.identity);
            Debug.Break();
            var yourObject = TheRing.GetComponent<RingCode>();
            yourObject.RingHurtSpawn(myVeloc);
        }
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
                DropRings();
                Infiniframes();
            }
        }
    }

    void Infiniframes()
    {
        //Putinvinble frames here
    }

    void NoRings()
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
}
