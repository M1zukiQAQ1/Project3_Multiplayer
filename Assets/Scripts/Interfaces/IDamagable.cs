using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public interface IDamagable
{
    public float TotalHealth { get; set; }
    public NetworkVariable<float> CurrentHealth { get; set; }
    
    [ServerRpc(RequireOwnership = false)]
    public void ReceiveDamageServerRpc(float damageReceived, NetworkObjectReference targetRef, ServerRpcParams serverRpcParams = default)
    {
        Debug.Log($"This code is running on host / server : {NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsServer}");

        targetRef.TryGet(out var targetObj);
        targetObj.GetComponent<IDamagable>().CurrentHealth.Value -= damageReceived;
        Debug.Log($"IDamageble: {targetRef.NetworkObjectId} received {damageReceived} damage, new current health is {targetObj.GetComponent<IDamagable>().CurrentHealth.Value}");
    }

    public void Death();

}
