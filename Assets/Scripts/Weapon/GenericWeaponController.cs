using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class GenericWeaponController : WeaponController
{
    public override void Fire()
    {
        if (timer >= (60 / firePerMinute) && currentNumberOfBullets > 0)
        {
            SpawnBulletServerRpc(owner.GetComponent<IWeaponHoldable>().GetFacingDirection());
            currentNumberOfBullets--;
            timer = 0;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    protected override void SpawnBulletServerRpc(Quaternion facingDirection)
    {
        var currentBullet = Instantiate(bulletPrefab, firePosition.position, facingDirection);
        Debug.Log("Weapon: Spawning Bullet" + currentBullet);
        
        currentBullet.GetComponent<NetworkObject>().SpawnWithOwnership(OwnerClientId);
        currentBullet.GetComponent<BulletController>().IntializeClientRpc(currentBullet.GetComponent<NetworkObject>().NetworkObjectId, bulletSpeed, damage, maximumDistance);
    }

    protected override void Update()
    {
        base.Update();
    }
}
