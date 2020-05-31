using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LedgeGrabState : PlayerState
{
    public override void Step(CharControlMotor player, float deltaTime)
    {
        if (player.HandleLedgeCheck())
        {
            player.state.ChangeState<WalkPlayerState>();
        }
    }

    public override void Enter(CharControlMotor player)
    {
        
    }
    public override void Exit(CharControlMotor player)
    {
        
    }
}
