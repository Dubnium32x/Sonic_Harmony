public class CrouchPlayerState : PlayerState
{
    public override void Enter(CharControlMotor player)
    {
        player.lookingDown = true;
        player.attacking = false;
        player.velocity.x = 0;
        player.ChangeBounds(1);
    }
    public override void Step(CharControlMotor player, float deltaTime)
    {
        player.HandleGravity(deltaTime);
        player.HandleFall();

        if (player.grounded && player.input.down)
        {
            if (player.input.actionDown)
            {
                player.state.ChangeState<SpindashPlayerState>();
            }
        }
        else
        {
            player.state.ChangeState<WalkPlayerState>();
        }
    }
    public override void Exit(CharControlMotor player)
    {
        player.lookingDown = false;
    }
}
