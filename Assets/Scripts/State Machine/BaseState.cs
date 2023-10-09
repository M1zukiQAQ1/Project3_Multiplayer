using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseState<T>
{
    protected StateMachine<T> stateMachine;
    protected T owner;

    public BaseState(StateMachine<T> stateMachine, T owner)
    {
        this.stateMachine = stateMachine;
        this.owner = owner;
    }

    public virtual void EnterState() { }
    public virtual void ExitState() { }
    public virtual void UpdateBehavior() { }
}
