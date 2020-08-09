using UnityEngine;

public class ImpatientState : PlayerState
{
    private float waitTime = 2.0f;
    private float timer = 0.0f;
    public override void Enter(CharControlMotor player)
    {
        player.Impatient = true;
    }

    public override void Step(CharControlMotor player, float deltaTime)
    {  
        timer += deltaTime;

        // Check if we have reached beyond 2 seconds.
        // Subtracting two is more accurate over time than resetting to zero.
        if (timer > waitTime)
        {
            player.state.ChangeState<WalkPlayerState>();
            // Remove the recorded 2 seconds.
            timer -= waitTime;
        }
        if (player.velocity.x > 0 || player.velocity.y > 0)
        {
            player.state.ChangeState<WalkPlayerState>();
        }
    }

    public override void Exit(CharControlMotor player)
    {
        player.Impatient = false;
    }
}