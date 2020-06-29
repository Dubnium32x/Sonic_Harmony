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
        active = false;
        charging = false;
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

            if(active)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                    charging = true;

                if (charging)
                {
                    dashTimer += Time.deltaTime * chargeSpeed;
                    dashSpeed.x = dashTimer;

                    if (Input.GetKeyUp(KeyCode.Space))
                    {
                        codedDashSpeed = dashSpeed;

                        if (player.input.left)
                            codedDashSpeed = codedDashSpeed - codedDashSpeed * 2;
                        else if (player.input.right)
                            codedDashSpeed = dashSpeed;
                        else
                        {
                            if (player.velocity.x < 0)
                                codedDashSpeed = codedDashSpeed - codedDashSpeed * 2;
                            else if (player.velocity.x > 0)
                                codedDashSpeed = dashSpeed;
                        }
                        active = false;
                        player.velocity = dashTarget.position - transform.position + codedDashSpeed;
                    }
                }
            }
            else if(Input.GetKeyUp(KeyCode.Space) && !charging)
                active = true;
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
    public bool charging = false;

    
}