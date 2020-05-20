using UnityEngine;

public abstract class PlayerState : MonoBehaviour
{
    public CharControlMotor.SonicState animationID;

    public virtual void Enter(CharControlMotor player) { }

    public virtual void Step(CharControlMotor player, float deltaTime) { }

    public virtual void Exit(CharControlMotor player) { }
}