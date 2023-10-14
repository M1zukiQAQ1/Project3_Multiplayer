using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Backpack/Weapon Item")]
public class WeaponItem : Item
{
    public GameObject weaponPrefab;
    
    public override void Use(PlayerController targetPlayer)
    {
        var weaponHolder = targetPlayer.GetComponent<IWeaponHoldable>();
        if(weaponHolder.CurrentWeapon != null)
        {
            Debug.LogWarning("Backpack: Player is holding a weapon, the old weapon will be destroyed");
            Destroy(weaponHolder.CurrentWeapon.gameObject);
            weaponHolder.CurrentWeapon = null;
        }
        ItemReference weaponRef = new(this);
        GameManager.instance.SpawnWeaponServerRpc(weaponRef);
    }
}