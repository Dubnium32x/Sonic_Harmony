using UnityEngine;

[AddComponentMenu("Freedom Engine/Objects/Item Box/Shield Box")]
public class SpeedShoeBox : ItemBox
{
    public PlayerShields shield;

    protected override void OnCollect(CharControlMotor player)
    {
        player.SpeedShoe = true;
        player.invincibleTimer = 15;

        // C U S T O M    H A R M O N Y    C O D E 
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
