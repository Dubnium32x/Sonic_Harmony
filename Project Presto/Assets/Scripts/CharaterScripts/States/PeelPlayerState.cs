using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.LowLevel;

public class PeelPlayerState : PlayerState
{
    public float power;
    public float chargeAmount;
    public float chargeSpeed;
    public float chargeNeededToShootOff;

    public override void Enter(CharControlMotor player)
    {
        chargeAmount = 0.0f;
        power = 0;
        player.attacking = true;
        //player.skin.ActiveBall(true);
        player.ChangeBounds(1);
        player.PlayAudio(player.audios.peel, 0.5f);

        // Move particle emission right
        if (player.direction == -1)
            player.particles.spindashSmoke.transform.localScale = new Vector3(-1, 1, 0);
        // Move particle emission left
        else
            player.particles.spindashSmoke.transform.localScale = new Vector3(1, 1, 0);
        player.particles.spindashSmoke.Play();
    }

    public override void Step(CharControlMotor player, float deltaTime)
    {
        player.HandleGravity(deltaTime);
        player.HandleFall();

        chargeAmount += Time.deltaTime * chargeSpeed;

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
                    //player.PlayAudio(player.audios.peel, 0.5f);
                }
            }
            else if (player.input.down)
            {
                if (Mathf.Abs(player.velocity.x) > player.stats.minSpeedToRoll)
                {
                    player.sonicState = CharControlMotor.SonicState.Rolling;
                    player.state.ChangeState<RollPlayerState>();
                    //player.PlayAudio(player.audios.spin, 0.5f);
                }
                else if (player.angle < player.stats.minAngleToSlide)
                {
                    player.state.ChangeState<CrouchPlayerState>();
                }
            }
            else
            {
                player.state.ChangeState<WalkPlayerState>();
            }
        }
    }

    public override void Exit(CharControlMotor player)
    {
        //player.skin.ActiveBall(false);
        if (chargeAmount >= chargeNeededToShootOff)
        {
            player.velocity.x = (player.stats.PeelminReleasePower + (Mathf.Floor(power) / 2)) * player.direction;
            player.PlayAudio(player.audios.peel_launch, 0.5f);
            player.sonicState = CharControlMotor.SonicState.Peel;
        }

        player.particles.spindashSmoke.Stop();
        player.disableSkinRotation = false;
    }
}
