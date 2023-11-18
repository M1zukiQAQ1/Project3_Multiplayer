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
    // TODO: Bug! Fix it! Use overlap sphere to judge if enemy can switch to fightstate
    public override void UpdateBehavior()
    {
        owner.NavAgent.SetDestination(owner.targetPlayer.position);
        // if (Physics.Raycast(new Ray(owner.transform.position, owner.transform.forward), out var hitInfo, owner.fireDistanceRadius))
        // {
        //     var playerObj = hitInfo.collider.GetComponentInParent<PlayerController>();
        //     if (playerObj != null && playerObj.GetComponent<NetworkObject>().NetworkObjectId == owner.targetPlayer.GetComponent<NetworkObject>().NetworkObjectId) 
        //     {
        //         stateMachine.ChangeState(owner.FightState);
        //         return;
        //     }

        // }
        Collider[] colliders = Physics.OverlapSphere(owner.transform.position, owner.fireDistanceRadius);
        foreach (var collider in colliders)
        {
            Debug.Log($"Enemy: {collider.GetComponentInParent<PlayerController>()} found, judging if they are the player");
            var currentPlayer = collider.GetComponentInParent<PlayerController>();

            if (currentPlayer != null)
            {
                Debug.Log($"Enemy: Comparing colliders' instance id. 1: {currentPlayer.transform.GetInstanceID()}; 2: {owner.targetPlayer.GetInstanceID()}");
                if (currentPlayer.transform.GetInstanceID() == owner.targetPlayer.GetInstanceID())
                {
                    Debug.Log("Enemy: Instance id is the same, changing to enemy's fightstate");
                    stateMachine.ChangeState(owner.FightState);
                }
            }
        }
    }
}
