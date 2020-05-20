public class HurtPlayerState : PlayerState
{

    public override void Enter(CharControlMotor player)
    {
        player.GroundExit();
        player.ChangeBounds(0);
        player.invincible = true;
        player.halfGravity = true;
        player.attacking = false;
        player.GotHurtCheck = true;
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
        player.invincibleTimer = player.stats.invincibleTime;
        player.GotHurtCheck = false;
    }
}
