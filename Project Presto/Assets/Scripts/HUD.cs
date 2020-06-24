using System.Collections;
using System.Text;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour {
    Canvas canvas;
    public Character character;
    Text livesText;
    Text ringsText;
    Text ringsTitleText;
    StringBuilder sb = new StringBuilder("", 50);

    Text scoreText;
    Text timeText;
    Text timeTitleText;

    void Awake() {
        canvas = GetComponent<Canvas>();
        scoreText = transform.Find("Score Content").GetComponent<Text>();
        timeText = transform.Find("Time Content").GetComponent<Text>();
        timeTitleText = transform.Find("Time Title").GetComponent<Text>();
        ringsText = transform.Find("Rings Content").GetComponent<Text>();
        ringsTitleText = transform.Find("Rings Title").GetComponent<Text>();
        livesText = transform.Find("Lives Content").GetComponent<Text>();
    }

    public void Update() {
        canvas.worldCamera = character.characterCamera.camera;
        // Debug.Log(character.characterCamera.camera);

        scoreText.text = Utils.IntToStrCached(character.score);
        ringsText.text = Utils.IntToStrCached(character.rings);
        livesText.text = Utils.IntToStrCached(character.lives);

        var minutes = (int)(character.timer / 60);
        var seconds = (int)(character.timer % 60);

        sb.Clear();
        sb.Append(Utils.IntToStrCached(minutes));
        sb.Append(":");
        if (seconds < 10) sb.Append("0");
        sb.Append(Utils.IntToStrCached(seconds));

        if (GlobalOptions.Get("timerType") != "NORMAL") {
            sb.Append(":");
            int preciseTime = 0;
            
            if (GlobalOptions.Get("timerType") == "CENTISECOND")
                preciseTime = (int)((character.timer % 1) * 100F);
            
            if (GlobalOptions.Get("timerType") == "FRAMES")
                preciseTime = (int)((character.timer * 60) % 60);

            if (preciseTime < 10) sb.Append("0");
            sb.Append(Utils.IntToStrCached(preciseTime));
        }
        
        timeText.text = sb.ToString();

        var shouldFlash = (((int)(Time.unscaledTime * 60)) % 16) > 8;
        if (shouldFlash) {
            if (character.rings <= 0) ringsTitleText.color = Color.red;

            if (!GlobalOptions.GetBool("timeLimit")) return;
            if (character.timer >= 9 * 60) timeTitleText.color = Color.red;
        } else {
            timeTitleText.color = Color.white;
            ringsTitleText.color = Color.white;
        }
    }
}
