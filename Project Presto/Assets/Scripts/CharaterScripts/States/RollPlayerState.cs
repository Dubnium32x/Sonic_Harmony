using UnityEngine;

public class RollPlayerState : PlayerState
{
	
    public override void Enter(CharControlMotor player)
    {
        player.attacking = true;
        player.particles.brakeSmoke.Play();
        player.disableSkinRotation = true;
        player.PlayAudio(player.audios.spindash, 0.5f);
    }
	
    public override void Step(CharControlMotor player, float deltaTime)
    {
        player.HandleSlopeFactor(deltaTime);
        player.HandleFriction(deltaTime);
        player.HandleDeceleration(deltaTime);
        player.HandleGravity(deltaTime);
        player.HandleFall();
		
		//Needs a math check ~Birb64
		
		
		if(player.PlayerObject.transform.eulerAngles.z > 110 && player.PlayerObject.transform.eulerAngles.z < 200)
		{
		player.PlayerObject.GetComponent<PlayerMotor>().height = 0.01f * player.PlayerObject.transform.eulerAngles.z;
		}
		if(player.PlayerObject.transform.eulerAngles.z < 250 && player.PlayerObject.transform.eulerAngles.z > 200)
		{
			player.PlayerObject.GetComponent<PlayerMotor>().height = 0.007f * player.PlayerObject.transform.eulerAngles.z;
		}
		
		
		
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
		player.PlayerObject.GetComponent<PlayerMotor>().height = 1;
    }
}