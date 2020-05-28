using UnityEngine;

public class BrakePlayerState : PlayerState
{
    public override void Enter(CharControlMotor player)
    {

        player.attacking = false;
        player.ChangeBounds(0);
        player.PlayAudio(player.audios.brake);
        player.UpdateDirection(player.velocity.x);
        //player.skin.SetEulerY(90 - player.direction * 90);
        //player.particles.brakeSmoke.Play();
    }

    public override void Step(CharControlMotor player, float deltaTime)
    {
        player.HandleDeceleration(deltaTime);
        player.HandleGravity(deltaTime);
        player.HandleFall();

        if (player.grounded && (Mathf.Sign(player.input.horizontal) != Mathf.Sign(player.direction)))
        {
            if (player.input.actionDown)
            {
                player.HandleJump();
            }
            else if ((Mathf.Abs(player.velocity.x) <= player.stats.turnSpeed) || player.input.horizontal == 0)
            {
                player.state.ChangeState<WalkPlayerState>();
            }
        }
        else
        {
            player.state.ChangeState<WalkPlayerState>();
        }
    }
    public override void Exit(CharControlMotor player)
    {
        //player.particles.brakeSmoke.Stop();
    }
}