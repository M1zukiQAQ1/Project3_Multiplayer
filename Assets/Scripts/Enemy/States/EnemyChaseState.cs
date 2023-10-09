using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.AI;

public class EnemyChaseState : BaseState<EnemyController>
{

    
    public EnemyChaseState(StateMachine<EnemyController> stateMachine, EnemyController owner) : base(stateMachine, owner)
    {
    }

    public override void EnterState()
    {
        //Update the destination when enemy entered chase state
        owner.NavAgent.enabled = true;
        owner.NavAgent.SetDestination(owner.targetPlayer.position);
    }

    public override void UpdateBehavior()
    {
        owner.NavAgent.SetDestination(owner.targetPlayer.position);
        if (Physics.Raycast(new Ray(owner.transform.position, owner.transform.forward), out var hitInfo, owner.fireDistanceRadius))
        {
            var playerObj = hitInfo.collider.GetComponentInParent<PlayerController>();
            if (playerObj != null && playerObj.GetComponent<NetworkObject>().NetworkObjectId == owner.targetPlayer.GetComponent<NetworkObject>().NetworkObjectId) 
            {
                stateMachine.ChangeState(owner.FightState);
                return;
            }

        }
    }
}
