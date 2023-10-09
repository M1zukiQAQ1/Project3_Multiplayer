using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class EnemyIdleState : BaseState<EnemyController>
{
    public EnemyIdleState(StateMachine<EnemyController> stateMachine, EnemyController owner) : base(stateMachine, owner)
    {
    }

    public override void EnterState()
    {
        Debug.Log($"Enemy: Enemy {owner.GetComponent<NetworkObject>().NetworkObjectId} successfully spawned and entered {ToString()}");
        owner.StartCoroutine(IEFindTargetPlayer());

        if (owner.isPatroller)
        {
            owner.NavAgent.SetDestination(owner.GetCurrentWayPoint());
        }
    }

    public override void ExitState()
    {
        base.ExitState();
    }

    public override void UpdateBehavior()
    {
        //Patrol among waypoints
        if (owner.isPatroller && owner.NavAgent.remainingDistance < owner.NavAgent.stoppingDistance)
        {
            var currentWaypoint = owner.GetCurrentWayPoint();
            Debug.Log($"Enemy: Enemy {owner.GetComponent<NetworkObject>().NetworkObjectId} reached one waypoint, updating its waypoint to {currentWaypoint}");
            owner.NavAgent.SetDestination(currentWaypoint);   
        }
    }

    private IEnumerator IEFindTargetPlayer()
    {
        Debug.Log($"Enemy: Enemy {owner.GetComponent<NetworkObject>().NetworkObjectId} is now finding target player");
        while (true)
        {
            yield return new WaitForSeconds(1f);
            Collider[] colliders = Physics.OverlapSphere(owner.transform.position, owner.visionRadius);
            foreach(Collider collider in colliders)
            {
                var target = collider.GetComponent<PlayerController>() != null ? collider.GetComponent<PlayerController>() : collider.GetComponentInParent<PlayerController>();
                if (target != null)
                {
                    Debug.Log($"Enemy: Enemy {owner.GetComponent<NetworkObject>().NetworkObjectId} found player {target.ClientId}, changing state to ChaseState");
                    owner.targetPlayer = target.transform;
                    stateMachine.ChangeState(owner.ChaseState);
                    yield break;
                }
            }
        }
        }

        public override string ToString() => "Base State";
}
