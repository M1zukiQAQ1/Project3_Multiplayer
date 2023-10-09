using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;

public class BulletController : NetworkBehaviour
{
    private float speed;
    private float maximumDistance;
    private float damage;

    private Vector3 spawnedLocation;

    private void Start()
    {
        spawnedLocation = transform.position;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
    }

    public void Update()
    {
        transform.Translate(speed * Time.deltaTime * Vector3.forward);
        
        if((transform.position - spawnedLocation).magnitude >= maximumDistance)
        {
            Debug.Log("Bullet: Reaching distance limit, " + transform.position + " " + spawnedLocation + ". Destroying gameobject!");
            GameManager.instance.DestroyNetworkObjectServerRpc(GetComponent<NetworkObject>());
        }
        
    }

    [ClientRpc]
    public void IntializeClientRpc(ulong objectId, float speed, float damage, float maximumDistance)
    {
        if (objectId != GetComponent<NetworkObject>().NetworkObjectId) return;
        this.damage = damage;
        this.maximumDistance = maximumDistance;
        this.speed = speed;

        Debug.Log("Bullet: Initializing bullet, its property: " + speed + " " + damage + " " + maximumDistance);
    }

    private void OnTriggerEnter(Collider other)
    {
        var targetRef = other.GetComponentInParent<NetworkObject>();
        if (targetRef != null)
        {
            Debug.Log("Bullet: Calling server to register hit, target reference is " + targetRef.NetworkObjectId);
            GameManager.instance.ReceiveDamageServerRpc(damage, targetRef);
        }
        Debug.Log("Bullet: Target is not a NetworkObject, returning");
        GameManager.instance.DestroyNetworkObjectServerRpc(this.GetComponent<NetworkObject>());
    }
}
