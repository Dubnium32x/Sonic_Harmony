using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LedgeGrabStateFront : PlayerState
{
    public override void Step(CharControlMotor player, float deltaTime)
    {
        player.UpdateDirection(player.input.horizontal);
        player.HandleSlopeFactor(deltaTime);
        player.HandleAcceleration(deltaTime);
        player.HandleFriction(deltaTime);
        player.HandleGravity(deltaTime);
        player.HandleFall();
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

        if (player.input.horizontal != 0 || player.velocity.x != 0)
        {
            player.state.ChangeState<WalkPlayerState>();
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
