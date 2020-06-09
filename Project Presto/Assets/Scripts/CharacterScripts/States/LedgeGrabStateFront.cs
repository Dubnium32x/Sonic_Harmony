using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LedgeGrabStateFront : PlayerState
{
    public override void Step(CharControlMotor player, float deltaTime)
    {
        if (!player.HandleLedgeCheck().Item1)
        {
            player.state.ChangeState<WalkPlayerState>();
        }
        if (player.HandleLedgeCheck().Item2)
        {
            player.state.ChangeState<LedgeGrabStateBack>();
        }
        if (player.input.actionDown)
        {
            player.HandleJump();
        }
        if (player.input.horizontal != 0)
        {
            if (Mathf.Abs(player.velocity.x) >= player.stats.minSpeedToBrake)
            {
                player.state.ChangeState<BrakePlayerState>();
            }
            else
            {
                if (player.velocity.x > 0)
                {
                    player.velocity.x = -player.stats.turnSpeed;
                }
                else
                {
                    player.velocity.x = player.stats.turnSpeed;
                }
            }
        }
    }

    public override void Enter(CharControlMotor player)
    {
        player.velocity.x = 0;
    }
    public override void Exit(CharControlMotor player)
    {
        
    }
}
