using System;
using UnityEngine;
using System.Collections.Generic;

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

            if (dashTimer > maxDashSpeed)
            {
                dashTimer = maxDashSpeed;
            }

            if(active)
            {
                if (Input.GetButtonDown("Jump"))
                {
                    player.PlayAudio(player.audios.spindash_charge, 0.5f);

                    charging = true;
                }
                    

                if (charging)
                {
                    dashTimer += Time.deltaTime * Time.timeScale;
                    player.HandleGravity(-deltaTime * 0.25f);
                    player.skin.animator.SetInteger("Charge", Mathf.FloorToInt((dashTimer / chargeTime) * 100));

                    if (Input.GetButtonUp("Jump"))
                    {
                        if(dashTimer < minChargeTime)
                        {
                            charging = false;
                        }
                        else
                        {
                            player.PlayAudio(player.audios.peel_launch, 0.5f);
                            float dashSpeed = Mathf.Lerp(0, maxDashSpeed, dashTimer / chargeTime);

                            if (player.input.left || (!player.input.right && player.velocity.x < 0) || (player.velocity.x.CompareTo(0) == 0 && player.direction < 0))
                                dashSpeed *= -1;

                            active = false;
                            if (Mathf.Abs(player.velocity.x) < player.stats.topSpeed)
                            {
                                player.velocity.x = Mathf.Clamp(player.velocity.x + dashSpeed, -player.stats.topSpeed, player.stats.topSpeed);
                                player.velocity.y = Mathf.Abs(dashSpeed) * dashVertAmount;
                            }
                            else
                            {
                                player.velocity.y = Mathf.Abs(dashSpeed) * maxSpeedDashVertAmount;
                            }
                        }

                        dashTimer = 0;
                        player.skin.animator.SetInteger("Charge", 0);
                    }
                }
            }
            else if(Input.GetButtonUp("Jump") && !charging)
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
    [Tooltip("Use the X-Axis only. (Changes Strenth of Dash)")]
    public float chargeTime;
    public float minChargeTime;
    public float maxDashSpeed;
    public float dashVertAmount;
    public float maxSpeedDashVertAmount;
    public bool active;
    public bool charging = false;
    
}