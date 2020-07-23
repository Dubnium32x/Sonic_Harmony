using UnityEngine;

public class RollPlayerState : PlayerState
{	
	public GameObject LePlayer;
    public override void Enter(CharControlMotor player)
    {
        player.attacking = true;		
        player.particles.brakeSmoke.Play();
        player.disableSkinRotation = true;
        player.PlayAudio(player.audios.spindash, 0.5f);
		
		LePlayer.GetComponent<PlayerMotor>().height = 2;
		LePlayer.GetComponent<PlayerMotor>().ChangeBounds(1);
    }

    public override void Step(CharControlMotor player, float deltaTime)
    {
        player.HandleSlopeFactor(deltaTime);
        player.HandleFriction(deltaTime);
        player.HandleDeceleration(deltaTime);
        player.HandleGravity(deltaTime);
        player.HandleFall();

        if (player.grounded)
        {
            if (player.input.actionDown)
            {                
                player.HandleJump();
            }
            else if (Mathf.Abs(player.velocity.x) < player.stats.minSpeedToUnroll)
            {
                player.state.ChangeState<WalkPlayerState>();
            }
        }
        else
        {
            player.state.ChangeState<JumpPlayerState>();
        }
    }

    public override void Exit(CharControlMotor player)
    {
        player.particles.brakeSmoke.Stop();
        player.disableSkinRotation = false;
		LePlayer.GetComponent<PlayerMotor>().height = 1;
    }
}