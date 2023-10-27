using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.VisualScripting;

public class GameManager : NetworkBehaviour
{
    public static GameManager instance;

    public ulong HostClientId { get; private set; }
    public int timeTillSelfDestruction = 300; // 5 mintues
    void Start()
    {
        HostClientId = NetworkManager.Singleton.LocalClientId;

        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void DestroyObjectWithChildren(Transform objectToDestroy)
    {
        Debug.Log($"GameManager: Destroying Object {objectToDestroy}");
        for (int i = objectToDestroy.transform.childCount - 1; i >= 0; i--)
        {
            if (objectToDestroy.transform.GetChild(i).childCount != 0)
            {
                DestroyObjectWithChildren(objectToDestroy.transform.GetChild(i));
            }
            else
            {
                objectToDestroy.transform.GetChild(i).gameObject.GetComponent<NetworkObject>().Despawn();
            }
        }
        objectToDestroy.GetComponent<NetworkObject>().Despawn();
    }

    [ServerRpc(RequireOwnership = false)]
    public void DestroyNetworkObjectServerRpc(NetworkObjectReference targetRef)
    {
        targetRef.TryGet(out var targetObj);
        DestroyObjectWithChildren(targetObj.transform);
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetClientHealthServerRpc(float targetHealth, NetworkObjectReference targetRef)
    {
        Debug.Log($"IDamagable: Setting health for object {targetRef.NetworkObjectId}");

        Debug.Log($"IDamagable: {targetRef.TryGet(out var targetObj)}");
        targetObj.GetComponent<IDamagable>().CurrentHealth.Value = targetHealth;
        Debug.Log($"IDamagable: {targetRef.NetworkObjectId}'s health is set to {targetObj.GetComponent<IDamagable>().CurrentHealth.Value}");
    }

    [ServerRpc(RequireOwnership = false)]
    public void StartLabSelfDestructServerRpc(){
        StartCoroutine(IEStartLabSelfDestruct());
    }

    private IEnumerator IEStartLabSelfDestruct(){
        yield return new WaitForSeconds(timeTillSelfDestruction);
        foreach (var player in FindObjectsOfType<PlayerController>())
        {
            DestroyNetworkObjectServerRpc(player.gameObject);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void ReceiveDamageServerRpc(float damageReceived, NetworkObjectReference targetRef, ServerRpcParams serverRpcParams = default)
    {
        Debug.Log($"This code is running on host / server : {NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsServer}");

        targetRef.TryGet(out var targetObj);
        Debug.Log($"GameManager: target object is {targetObj}");

        if(targetObj.GetComponent<IDamagable>() == null)
        {
            Debug.Log($"GameManager: object is not damagable, returning!");
            return;
        }

        targetObj.GetComponent<IDamagable>().CurrentHealth.Value -= damageReceived;
        Debug.Log($"IDamageble: {targetRef.NetworkObjectId} received {damageReceived} damage, new current health is {targetObj.GetComponent<IDamagable>().CurrentHealth.Value}");
    }

    [ClientRpc]
    public void SetWeaponRefClientRpc(ulong _clientId, NetworkObjectReference _weaponRef)
    {
        Debug.Log($"Weapon: Set weapon called");
        var clientId = NetworkManager.Singleton.LocalClientId;
        if (_clientId != clientId)
        {
            Debug.Log("Player: Local client " + clientId + " doesn't match targeted id " + _clientId + ", returning.");
            return;
        }

        Debug.Log("Weapon: Setting reference for client " + clientId);
        _weaponRef.TryGet(out var _weaponObj);
        NetworkManager.LocalClient.PlayerObject.GetComponent<PlayerController>().CurrentWeapon = _weaponObj.GetComponent<WeaponController>();
        _weaponObj.GetComponent<WeaponController>().owner = NetworkManager.LocalClient.PlayerObject.GetComponent<PlayerController>().transform;
    }

    [ServerRpc(RequireOwnership = false)]
    public void SpawnWeaponServerRpc(ItemReference itemRef, ServerRpcParams serverRpcParams = default)
    {
        ulong _clientId = serverRpcParams.Receive.SenderClientId;
        var clientObj = NetworkManager.Singleton.ConnectedClients[_clientId].PlayerObject.GetComponent<PlayerController>();

        if (itemRef.TryResolve(out Item weaponItem))
        {
            var currentWeapon = Instantiate((weaponItem as WeaponItem).weaponPrefab);
            currentWeapon.GetComponent<NetworkObject>().SpawnWithOwnership(_clientId);
            SetWeaponRefClientRpc(_clientId, currentWeapon.GetComponent<NetworkObject>());

            Debug.Log("Weapon: Setting parent for client " + _clientId + ", returning " + currentWeapon.GetComponent<NetworkObject>().TrySetParent(clientObj.weaponHoldPos));
        }
        else
        {
            Debug.LogError("Player: Weapon didn't resolved! is the reference for weapon prefab missing?");
        }
    }

    public PlayerController GetPlayerOwnedByClient()
    {
        PlayerController[] players = FindObjectsOfType<PlayerController>();
        foreach (PlayerController player in players)
        {
            if (player.GetIsOwner() == true)
            {
                return player;
            }
        }

        Debug.LogWarning("Cannot find player owned by this client, is the server and client started?");
        return null;
    }
}
