using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;

public class EnemyFightState : BaseState<EnemyController>
{
    private const float allowedDistance = 0.5f;
    public EnemyFightState(StateMachine<EnemyController> stateMachine, EnemyController owner) : base(stateMachine, owner)
    {
    }

    public override void EnterState()
    {
        owner.NavAgent.enabled = false;
    }

    public override void ExitState()
    {
        owner.NavAgent.enabled = true;
    }

    public override void UpdateBehavior()
    {
        if (!owner.CurrentWeapon.Fire())
        {
            owner.CurrentWeapon.Reload();
        }

        // Make enemy rotate towards the targeted player
        // owner.transform.LookAt(owner.targetPlayer);
        var direction = (owner.targetPlayer.position - owner.transform.position).normalized;
        var targetRotation = Quaternion.LookRotation(direction);
        owner.transform.rotation = Quaternion.Lerp(owner.transform.rotation, targetRotation, Time.deltaTime * owner.rotateSpeed);

        if((owner.transform.position - owner.targetPlayer.position).magnitude > owner.fireDistanceRadius + allowedDistance)
        {
            stateMachine.ChangeState(owner.ChaseState);
        }

    }
}
