using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyController : NetworkBehaviour, IDamagable, IWeaponHoldable
{
    private StateMachine<EnemyController> enemyStateMachine = new();
    public EnemyIdleState IdleState { get; private set; }
    public EnemyChaseState ChaseState { get; private set; }
    public EnemyFightState FightState { get; private set; }

    public event Action OnEnemyDeath;

    [Header("Patrolling")]
    [SerializeField] private NavMeshAgent navAgent;
    public NavMeshAgent NavAgent
    {
        get => navAgent;
        private set
        {
            navAgent = value;
        }
    }
    public bool isPatroller;
    public List<Vector3> wayPoints;
    
    [Header("Combat")]
    public float visionRadius = 10f;
    public float fireDistanceRadius = 5f;
    [SerializeField] private float totalHealth;
    public float TotalHealth
    {
        get => totalHealth;
        set
        {
            totalHealth = value;
        }
    }

    public NetworkVariable<float> CurrentHealth { get; set; } = new(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    
    public WeaponController CurrentWeapon 
    {
        get => currentWeapon;
        set
        {
            currentWeapon = value;
        }
    }
    [SerializeField] private WeaponController currentWeapon;
    public ulong ClientId { get; set; } = 0;
    public Transform targetPlayer;

    private uint currentWaypointIndex = 0;

    void Start()
    {

        navAgent = FindObjectOfType<NavMeshAgent>();
        IdleState = new(enemyStateMachine, this);
        ChaseState = new(enemyStateMachine, this);
        FightState = new(enemyStateMachine, this);
        enemyStateMachine.Initialize(IdleState);

        OnEnemyDeath += Death;
    }

    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
            return;

        Debug.Log($"Enemy: Enemy {GetComponent<NetworkObject>().NetworkObjectId} is executing onNetworkSpawned");
        GameManager.instance.SetClientHealthServerRpc(totalHealth, GetComponent<NetworkObject>());
        CurrentHealth.OnValueChanged += (float previous, float current) =>
        {
            if (current <= 0) OnEnemyDeath.Invoke();
        };
        CurrentHealth.OnValueChanged += (float previous, float current) =>
        {
            Debug.Log($"Enemy: Enemy {GetComponent<NetworkObject>().NetworkObjectId} is invoking DisplayDamageNumber method");
            ClientUIController.instance.DisplayDamageNumberClientRpc(previous - current, GetComponent<NetworkObject>());
        };

        base.OnNetworkSpawn();
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner)
            return;
        enemyStateMachine.currentState.UpdateBehavior();
    }

    public Vector3 GetCurrentWayPoint() => wayPoints[(int)++currentWaypointIndex % wayPoints.Count];

    public void Death()
    {
        Debug.Log($"Enemy: Enemy {GetComponent<NetworkObject>().NetworkObjectId} died, destroying GameObject");
        GameManager.instance.DestroyNetworkObjectServerRpc(GetComponent<NetworkObject>());
    }

    public Quaternion GetFacingDirection()
    {
        Debug.Log($"Enemy: Enemy {GetComponent<NetworkObject>().NetworkObjectId} is getting its facing direction, transform.rotation eqauls {transform.rotation}");
        return transform.rotation;
    }

    public Transform GetWeaponHoldPosition()
    {
        throw new System.NotImplementedException();
    }
}
