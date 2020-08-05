using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Text;

[AddComponentMenu("Freedom Engine/Game/Score Manager")]
public class ScoreManager : MonoBehaviour
{
	private static ScoreManager instance;

	private static readonly string[] digits = new string[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", ":", "." };
	private static readonly StringBuilder timer = new StringBuilder(8);
	[SerializeField] private Image fader = null;

	[Header("Game Over UI")]
	[SerializeField] private float gameOverFadeTime = 1f;

	[SerializeField] private AudioClip gameOverJingle = null;
	[SerializeField] private string gameOverScene = "";
	[SerializeField] private GameObject gameOverUI = null;
	[SerializeField] private Text lifeCounter = null;
	private int lifes;
	private int minutes;
	[SerializeField] private Text ringCounter = null;

	private int rings;

	private int seconds;

	[Header("Score UI")]
	[SerializeField] private Text timeCounter = null;

	public static ScoreManager Instance
	{
		get
		{
			if (instance != null) return instance;
			instance = FindObjectOfType<ScoreManager>();
			instance.StartSingleton();

			return instance;
		}
	}

	public float time { get; set; }
	public bool stopTimer { get; set; }

	public int Rings
	{
		get { return rings; }
		set 
		{
			rings = value;
			ringCounter.text = rings.ToString("D3");
		}
	}

	public int Lifes
	{
		get { return lifes; }
		set
		{
			lifes = value;
			lifeCounter.text = lifes.ToString("D2");
		}
	}

	private void StartSingleton()
	{
		Lifes = 3;
		time = 0;
	}

	private void Update()
	{
		if (stopTimer) return;
		time += Time.deltaTime;

		minutes = (int)(time / 60f) % 60;
		seconds = (int)(time % 60);
			
		timer.Length = 0;
		timer.Append(digits[minutes / 10]);
		timer.Append(digits[minutes % 10]);
		timer.Append(digits[10]);
		timer.Append(digits[seconds / 10]);
		timer.Append(digits[seconds % 10]);
		timeCounter.text = timer.ToString();
		if (minutes <= 10 && seconds > 30)
		{
			if (timeCounter.color == Color.white)
			{
				timeCounter.color = Color.red;
			}

			if (timeCounter.color == Color.red)
			{
				timeCounter.color = Color.white;
			}
		}
	}

	public void ResetTimer()
	{
		time = minutes = seconds = 0;
		timeCounter.text = "00:00";
	}

	public void Die()
	{
		stopTimer = true;

		if (lifes > 0)
		{
			StageManager.Instance.Restart();
		}
		else
		{
			StartCoroutine(GameOver());
		}
	}

	private IEnumerator GameOver()
	{
		gameOverUI.SetActive(true);
		StageManager.Instance.ChangeSong(gameOverJingle, 0.5f);

		yield return new WaitForSeconds(gameOverJingle.length);

		var elapsedTime = 0f;
		var faderColor = fader.color;

		while (elapsedTime < gameOverFadeTime)
		{
			elapsedTime += Time.deltaTime;

			var alpha = elapsedTime / gameOverFadeTime;
			faderColor.a = Mathf.Lerp(0, 1, alpha);
			fader.color = faderColor;

			yield return null;
		}

		SceneManager.LoadScene(gameOverScene);
	}
}
