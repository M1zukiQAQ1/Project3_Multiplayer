using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFightState : BaseState<EnemyController>
{
    public EnemyFightState(StateMachine<EnemyController> stateMachine, EnemyController owner) : base(stateMachine, owner)
    {
    }

    public override void EnterState()
    {
        owner.NavAgent.enabled = false;
    }

    public override void UpdateBehavior()
    {
        if (!owner.CurrentWeapon.Fire())
        {
            owner.CurrentWeapon.Reload();
        }   

        if((owner.transform.position - owner.targetPlayer.position).magnitude > owner.fireDistanceRadius)
        {
            stateMachine.ChangeState(owner.ChaseState);
        }

    }
}
