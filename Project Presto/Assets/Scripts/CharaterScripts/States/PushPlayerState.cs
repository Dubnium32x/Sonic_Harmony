using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushPlayerState : PlayerState
{

    public override void Enter(CharControlMotor player)
    {
    }

    public override void Step(CharControlMotor player, float deltaTime)
    {
        var PushResult = player.HandlePushCheck();
        if (PushResult.Item1)
        {
            if (player.input.actionDown)
            {
                player.HandleJump();
            }
            //apply push to physical object
            if (player.input.horizontal != 0 && PushResult.gameObject.CompareTag("PhysicalObject"))
            {
                var rigidbody = PushResult.gameObject.GetComponent<Rigidbody>();
                rigidbody.AddForce(player.skin.transform.right.normalized * player.input.horizontal);
            }

            if (Mathf.Sign(player.velocity.x) != Mathf.Sign(player.input.horizontal))
            {
                player.state.ChangeState<WalkPlayerState>();
            }
        }
        else
        {
            player.state.ChangeState<WalkPlayerState>();
        }
    }
}
