using UnityEngine;

public class DiePlayerState : PlayerState
{
    public float deathUpwardForceAmount;

    public override void Enter(CharControlMotor player)
    {
        player.GroundExit();
        player.EnableCollision(false);
        player.attacking = false;
        player.Death = true;
        player.disableSkinRotation = true;
        player.disableCameraFollow = true;
        player.velocity = Vector3.zero;
		player.velocity.y = deathUpwardForceAmount;
        player.PlayAudio(player.audios.death);
    }
    public override void Step(CharControlMotor player, float deltaTime)
    {
        player.HandleGravity(deltaTime);
    }

    public override void Exit(CharControlMotor player)
    {
        player.Death = false;
    }
}