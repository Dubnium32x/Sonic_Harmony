using System;
using UnityEngine;
using System.Collections.Generic;

public class JumpPlayerState : PlayerState
{

    public float jumpSlowDownAmount;

    public override void Enter(CharControlMotor player)
    {
        player.attacking = true;
        player.jumped = true;
        player.ChangeBounds(1);

        dashTimer = 0.0f;
        active = false;
        charging = false;

        jumpSlowDownAmount = player.velocity.x / 8;

        if (player.velocity.x != 0)
        {
            if (player.velocity.x < 0)
            {
                player.velocity.x = player.velocity.x - jumpSlowDownAmount;
            }
            else if (player.velocity.x > 0)
            {
                player.velocity.x = player.velocity.x - jumpSlowDownAmount;
            }
        }
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
                {
                    player.PlayAudio(player.audios.spindash_charge, 0.5f);

                    charging = true;
                }
                    

                if (charging)
                {
                    dashTimer += Time.deltaTime * chargeSpeed;
                    dashSpeed.x = dashTimer;

                    if (Input.GetKeyUp(KeyCode.Space))
                    {
                        codedDashSpeed = dashSpeed;
                        player.PlayAudio(player.audios.peel_launch, 0.5f);

                        if (player.input.left)
                        {
                            codedDashSpeed -= codedDashSpeed * 2;
                        }
                        else if (player.input.right)
                        {
                            codedDashSpeed = dashSpeed;
                        }
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
            {
                active = true;
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
    public bool charging = false;

    
}