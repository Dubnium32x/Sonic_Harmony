using UnityEngine;

public class RollPlayerState : PlayerState
{
	public GameObject playermotor;
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
		/*
		
		if (playermotor.transform.eulerAngles.z > 1 && playermotor.transform.eulerAngles.z < 181)
		{
		playermotor.GetComponent<PlayerMotor>().height = playermotor.transform.eulerAngles.z;
		}
		
		if (playermotor.transform.eulerAngles.z < 360 && playermotor.transform.eulerAngles.z > 179)
		{
		playermotor.GetComponent<PlayerMotor>().height = playermotor.transform.eulerAngles.z;
		}
		
		*/
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
		playermotor.GetComponent<PlayerMotor>().height = 1;
    }
}