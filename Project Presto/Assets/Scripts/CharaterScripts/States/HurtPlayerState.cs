public class HurtPlayerState : PlayerState
{
    public float upwardForceAmount;

    public override void Enter(CharControlMotor player)
    {
        player.GroundExit();
        player.ChangeBounds(0);
        player.invincible = true;
        player.halfGravity = true;
        player.attacking = false;
        player.GotHurtCheck = true;
        player.velocity.y = upwardForceAmount;
    }
    public override void Step(CharControlMotor player, float deltaTime)
    {
        player.HandleGravity(deltaTime);

        if (player.grounded)
        {
            player.velocity.x = 0;
            player.state.ChangeState<WalkPlayerState>();
        }
    }
    public override void Exit(CharControlMotor player)
    {
        player.halfGravity = false;
        player.skin.StartBlinking(player.stats.invincibleTime);
        player.invincibleTimer = player.stats.invincibleTime;
    }
}
