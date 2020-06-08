using UnityEngine;

[AddComponentMenu("Freedom Engine/Objects/Item Box/Shield Box")]
public class SpeedShoeBox : ItemBox
{
    public PlayerShields shield;

    protected override void OnCollect(CharControlMotor player)
    {
        player.SetShield(shield);
    }
}
