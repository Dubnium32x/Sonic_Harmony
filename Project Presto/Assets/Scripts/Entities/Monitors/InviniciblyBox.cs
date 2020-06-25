using UnityEngine;

[AddComponentMenu("Freedom Engine/Objects/Item Box/OneUp")]
public class InviniciblyBox : ItemBox
{
	protected override void OnCollect(CharControlMotor player)
	{
		player.invincible = true;
		player.invincibleTimer = 15;
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
