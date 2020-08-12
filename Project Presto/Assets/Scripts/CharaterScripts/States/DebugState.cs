using System;
using UnityEngine;
using UnityEditor;
public class DebugState : PlayerState
{///Simple Dimple Debug System SDDS by Birb64

	private Transform Player; //Read-only
	

	
	public override void Enter(CharControlMotor player)
	{
		player.EnableCollision(false);
	}
	
    public override void Step(CharControlMotor player, float deltaTime)
    {
		Player = player.PlayerObject.transform;

			

        player.HandleAcceleration(deltaTime);
		
			player.velocity = Vector3.zero;
			
			if(player.input.right)
			{
				player.velocity = Vector3.right * 20;
			}
			if(player.input.left)
			{
				player.velocity = Vector3.left * 20;
			}
			if(player.input.up)
			{
				player.velocity = Vector3.up * 20;
			}
			if(player.input.down)
			{
				player.velocity = Vector3.down * 20;
			}
    }
	public override void Exit(CharControlMotor player)
	{

		player.EnableCollision(true);
	}

}
