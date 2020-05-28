using UnityEngine;

public class SpindashPlayerState : PlayerState
{
    private float power;


    public override void Enter(CharControlMotor player)
    {
        power = 0;
        player.attacking = true;
        //player.skin.ActiveBall(true);
        player.ChangeBounds(1);
        player.PlayAudio(player.audios.spindash_charge, 0.5f);
        //player.particles.spindashSmoke.Play();
    }

    public override void Step(CharControlMotor player, float deltaTime)
    {
        player.HandleGravity(deltaTime);
        player.HandleFall();

        power -= ((power / player.stats.powerLoss) / 256f) * deltaTime;

        if (player.grounded)
        {
            if (player.input.down)
            {
                if (player.input.actionDown)
                {
                    player.sonicState = CharControlMotor.SonicState.ChargingSpin;
                    power += player.stats.chargePower;
                    power = Mathf.Min(power, player.stats.maxChargePower);
                    player.PlayAudio(player.audios.spindash_charge, 0.5f);
                }
            }
            else
            {
                player.sonicState = CharControlMotor.SonicState.Rolling;
                player.state.ChangeState<RollPlayerState>();
            }
        }
    }

    public override void Exit(CharControlMotor player)
    {
        //player.skin.ActiveBall(false);
        player.velocity.x = (player.stats.minReleasePower + (Mathf.Floor(power) / 2)) * player.direction;
        player.PlayAudio(player.audios.spindash, 0.5f);
        //player.particles.spindashSmoke.Stop();
    }
}
