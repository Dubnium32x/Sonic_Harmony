using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeelPlayerState : PlayerState
{
    private float power;


    public override void Enter(CharControlMotor player)
    {
        power = 0;
        player.attacking = true;
        //player.skin.ActiveBall(true);
        player.ChangeBounds(1);
        player.PlayAudio(player.audios.spindash_charge, 0.5f);
        player.particles.spindashSmoke.Play();
    }

    public override void Step(CharControlMotor player, float deltaTime)
    {
        player.HandleGravity(deltaTime);
        player.HandleFall();

        power -= ((power / player.stats.PeelpowerLoss) / 256f) * deltaTime;

        if (player.grounded)
        {
            if (player.input.up)
            {
                if (player.input.actionDown)
                {
                    player.sonicState = CharControlMotor.SonicState.ChargingPeel;
                    power += player.stats.PeelchargePower;
                    power = Mathf.Min(power, player.stats.PeelmaxChargePower);
                    player.PlayAudio(player.audios.peel, 0.5f);
                }
            }
            else
            {
                player.state.ChangeState<RollPlayerState>();
            }
        }
    }

    public override void Exit(CharControlMotor player)
    {
        //player.skin.ActiveBall(false);
        player.velocity.x = (player.stats.PeelminReleasePower + (Mathf.Floor(power) / 2)) * player.direction;
        player.PlayAudio(player.audios.peel_launch, 0.5f);
        //player.particles.spindashSmoke.Stop();
    }
}
