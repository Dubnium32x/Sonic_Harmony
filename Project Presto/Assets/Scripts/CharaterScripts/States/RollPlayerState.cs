using UnityEngine;

public class RollPlayerState : PlayerState
{
	
    public override void Enter(CharControlMotor player)
    {
        player.attacking = true;
        player.particles.brakeSmoke.Play();
        player.disableSkinRotation = true;
        player.PlayAudio(player.audios.spindash, 0.5f);
		player.PlayerObject.GetComponent<PlayerMotor>().ChangeBounds(1);
    }
	
    public override void Step(CharControlMotor player, float deltaTime)
    {
        player.HandleSlopeFactor(deltaTime);
        player.HandleFriction(deltaTime);
        player.HandleDeceleration(deltaTime);
        player.HandleGravity(deltaTime);
        player.HandleFall();
		
		
		///* Controls the height of the playerspin ~Birb64 *///
		
		if(player.PlayerObject.transform.eulerAngles.z > 100 && player.PlayerObject.transform.eulerAngles.z < 180)
		{
		player.PlayerObject.GetComponent<PlayerMotor>().height = 0.012f * player.PlayerObject.transform.eulerAngles.z;
		}
		if(player.PlayerObject.transform.eulerAngles.z < 260 && player.PlayerObject.transform.eulerAngles.z > 180)
		{
			player.PlayerObject.GetComponent<PlayerMotor>().height = 0.012f * (180 - Mathf.Abs(player.PlayerObject.transform.eulerAngles.z - 180));
		}
		
		///* Controls the width of the playerspin ~Birb64 *///
		if(player.PlayerObject.transform.eulerAngles.z > 30 && player.PlayerObject.transform.eulerAngles.z < 180)
		{
		player.PlayerObject.GetComponent<PlayerMotor>().wallExtents = 0.01f * player.PlayerObject.transform.eulerAngles.z;
		}
		if(player.PlayerObject.transform.eulerAngles.z < 330 && player.PlayerObject.transform.eulerAngles.z > 180)
		{
			player.PlayerObject.GetComponent<PlayerMotor>().wallExtents = 0.01f * (180 - Mathf.Abs(player.PlayerObject.transform.eulerAngles.z - 180));
		}
		else if (player.PlayerObject.transform.eulerAngles.z < 180 || player.PlayerObject.transform.eulerAngles.z > 260)
		{
			player.PlayerObject.GetComponent<PlayerMotor>().wallExtents = 0.02f;
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
		player.PlayerObject.GetComponent<PlayerMotor>().ChangeBounds(0);
    }
}