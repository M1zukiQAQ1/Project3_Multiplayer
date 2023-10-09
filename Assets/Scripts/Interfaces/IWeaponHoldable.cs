using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public interface IWeaponHoldable
{
    public Quaternion GetFacingDirection();
    public Transform GetWeaponHoldPosition();
    public WeaponController CurrentWeapon { get; set; }

    // If enemy is holding weapon, leave this blank
    public ulong ClientId { get; }
}
