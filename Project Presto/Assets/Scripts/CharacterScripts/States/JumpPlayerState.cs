public class JumpPlayerState : PlayerState
{

    public override void Enter(CharControlMotor player)
    {
        player.attacking = true;
        player.jumped = true;
        player.ChangeBounds(1);
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
}