using System;
using UnityEngine;

public class JumpPlayerState : PlayerState
{

    public override void Enter(CharControlMotor player)
    {
        player.attacking = true;
        player.jumped = true;
        player.ChangeBounds(1);

        dashTimer = 0.0f;
        active = true;
    }


    public override void Step(CharControlMotor player, float deltaTime)
    {
        player.UpdateDirection(player.input.horizontal);
        player.HandleAcceleration(deltaTime);
        player.HandleGravity(deltaTime);

        

        if (!player.grounded && player.attacking)
        {
            if (player.input.actionUp && player.velocity.y > player.stats.minJumpHeight)
            {
                player.velocity.y = player.stats.minJumpHeight;
            }

            if (dashTimer > maxDashAmount)
            {
                dashTimer = maxDashAmount;
            }

            if (Input.GetKeyDown(KeyCode.L))
            {
                isCharging = true;
            }


            if (isCharging == true)
            {
                dashTimer += Time.deltaTime * chargeSpeed;
                dashSpeed.x = dashTimer;
            }


            if (Input.GetKeyUp(KeyCode.L) && active == true)
            {
                codedDashSpeed = dashSpeed;

                

                if (player.input.left)
                {
                    codedDashSpeed = codedDashSpeed - codedDashSpeed * 2;
                    player.velocity = dashTarget.position - transform.position + codedDashSpeed;
                    active = false;
                }
                else if (player.input.right)
                {
                    codedDashSpeed = dashSpeed;
                    player.velocity = dashTarget.position - transform.position + codedDashSpeed;
                    active = false;
                }

                
            }

        }
        else
        {
            player.state.ChangeState<WalkPlayerState>();
        }
    }

    public override void Exit(CharControlMotor player)
    {
        player.jumped = false;
    }

    [Header("L is the Debug key for the Slingshot/ChargeDash/CloudBurst")]
    public float dashTimer = 0.0f;
    public float dashTimerStartTime;
    public Transform dashTarget;
    [Tooltip("Use the X-Axis only. (Changes Strenth of Dash)")]
    public Vector3 dashSpeed;
    public float chargeSpeed;
    public float maxDashAmount;
    public Vector3 codedDashSpeed;
    public bool active;
    public bool isCharging = false;

    
}