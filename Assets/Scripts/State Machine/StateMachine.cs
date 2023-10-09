using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine<T>
{
    public BaseState<T> currentState;
    
    public void Initialize(BaseState<T> enterState)
    {
        currentState = enterState;
        currentState.EnterState();
    }

    public void ChangeState(BaseState<T> nextState)
    {
        Debug.Log($"State Machine: Changing state from {currentState} to {nextState}");
        currentState.ExitState();
        currentState = nextState;
        nextState.EnterState();
    }

}
