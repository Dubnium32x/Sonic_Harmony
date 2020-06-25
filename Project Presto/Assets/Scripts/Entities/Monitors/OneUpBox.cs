using UnityEngine;

[AddComponentMenu("Freedom Engine/Objects/Item Box/OneUp")]
public class OneUpBox : ItemBox
{
	protected override void OnCollect(CharControlMotor player)
	{
		ScoreManager.Instance.Lifes++;
        FindObjectOfType<StageManager>().StopMusic();
        Invoke("UnpauseMusic", TimeUntilMusicStartsAgain);
    }

    // C U S T O M    H A R M O N Y    C O D E 
    public float TimeUntilMusicStartsAgain;

    void UnpauseMusic()
    {
        FindObjectOfType<StageManager>().ContinueMusic();
    }

}
