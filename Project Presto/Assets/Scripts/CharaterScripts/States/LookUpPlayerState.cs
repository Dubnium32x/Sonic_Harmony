public class LookUpPlayerState : PlayerState
{

    public override void Enter(CharControlMotor player)
    {
        player.lookingUp = true;
        player.attacking = false;
        player.velocity.x = 0;
        player.ChangeBounds(1);
    }

    public override void Step(CharControlMotor player, float deltaTime)
    {
        player.HandleGravity(deltaTime);
        player.HandleFall();

        if (player.grounded)
        {
            if (!player.input.up)
            {
                player.state.ChangeState<WalkPlayerState>();
            }
            else if (player.input.actionDown)
            {
                player.state.ChangeState<PeelPlayerState>();
            }
        }
        else
        {
            player.state.ChangeState<WalkPlayerState>();
        }
    }

    public override void Exit(CharControlMotor player)
    {
        player.lookingUp = false;
    }
}
