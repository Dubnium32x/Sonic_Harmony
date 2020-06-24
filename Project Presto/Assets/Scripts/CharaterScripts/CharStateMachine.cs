using System;
using System.Collections.Generic;

public class CharStateMachine
{
    private readonly CharControlMotor player;

    private readonly Dictionary<Type, PlayerState> states = new Dictionary<Type, PlayerState>();

    private Type currentState;

    public CharStateMachine(CharControlMotor player)
    {
        this.player = player;
    }

    public CharControlMotor.SonicState stateId
    {
        get
        {
            var animationId = states[currentState].animationID;
            return animationId;
        }
    }

    public void AddState(PlayerState state)
    {
        var type = state.GetType();

        if (states.ContainsKey(type)) return;
        states.Add(type, state);
    }

    public void ChangeState<T>() where T : PlayerState
    {
        var type = typeof(T);

        if (!states.ContainsKey(type)) return;
        if (currentState != null)
        {
            states[currentState].Exit(player);
        }
        currentState = type;
        states[currentState].Enter(player);
    }

    public void UpdateState(float deltaTime)
    {
        if (currentState == null) return;

        states[currentState].Step(player, deltaTime);
    }
}