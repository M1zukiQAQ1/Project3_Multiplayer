using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Backpack/Weapon Item")]
public class WeaponItem : Item
{
    public GameObject weaponPrefab;
    
    public void InstantiateWeaponWithOwnership(IWeaponHoldable objectHolder)
    {
        if(objectHolder.CurrentWeapon != null)
        {
            Debug.LogWarning("Backpack: Player is holding a weapon, the old weapon will be destroyed");
            Destroy(objectHolder.CurrentWeapon.gameObject);
            objectHolder.CurrentWeapon = null;
        }
        ItemReference weaponRef = new(this);
        GameManager.instance.SpawnWeaponServerRpc(weaponRef);
    }

    public void InstantiateWeaponWithoutOwnerShip(IWeaponHoldable holdingObject)
    {
        throw new System.NotImplementedException();
    }

}