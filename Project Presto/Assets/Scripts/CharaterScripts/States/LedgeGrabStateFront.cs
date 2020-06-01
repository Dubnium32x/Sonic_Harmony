using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LedgeGrabStateFront : PlayerState
{
    public override void Step(CharControlMotor player, float deltaTime)
    {
        if (player.HandleLedgeCheck().Item1)
        {
            player.state.ChangeState<WalkPlayerState>();
        }
        if (player.input.actionDown)
        {
            player.HandleJump();
        }
    }

    public override void Enter(CharControlMotor player)
    {
        
    }
    public override void Exit(CharControlMotor player)
    {
        
    }
}
