using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFightState : BaseState<EnemyController>
{
    public EnemyFightState(StateMachine<EnemyController> stateMachine, EnemyController owner) : base(stateMachine, owner)
    {
    }

    public override void UpdateBehavior()
    {
        /*
        if((owner.targetPlayer.position - owner.transform.position).magnitude > owner.fireDistanceRadius)
        {
            stateMachine.ChangeState(owner.ChaseState);
            return;
        }
        */
        owner.CurrentWeapon.Fire();
        owner.NavAgent.enabled = false;

        if((owner.transform.position - owner.targetPlayer.position).magnitude > owner.fireDistanceRadius)
        {
            stateMachine.ChangeState(owner.ChaseState);
        }

    }
}
