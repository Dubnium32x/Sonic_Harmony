public class SpringState : PlayerState
{

    public override void Enter(CharControlMotor player)
    {
        player.attacking = false;
        player.ChangeBounds(0);
        player.UpdateDirection(player.velocity.x);
    }

    public override void Step(CharControlMotor player, float deltaTime)
    {
        player.HandleAcceleration(deltaTime);
        player.HandleGravity(deltaTime);

        if (player.velocity.y <= 0)
        {
            player.state.ChangeState<WalkPlayerState>();
        }
    }
}
