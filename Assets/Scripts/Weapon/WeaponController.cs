using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Netcode;

public abstract class WeaponController : NetworkBehaviour
{
    [Header("References")]
    public GameObject bulletPrefab;
    public Transform owner;
    public Transform firePosition;

    [Header("Weapon Attributes")]
    [SerializeField] protected float bulletSpeed;
    [SerializeField] protected float damage;
    [SerializeField] protected float maximumDistance;
    [SerializeField] protected float firePerMinute;
    [SerializeField] protected int bulletPerMagazine;
    public int currentNumberOfBullets;
    
    protected float timer;
    
    // currentBullets, bulletsInOneMagazine
    protected event Action<int, int> onWeaponFired;

    private void Start()
    { 
        transform.localPosition = Vector3.zero;
        transform.localRotation = new Quaternion(0, 0, 0, 0);

        onWeaponFired += FireInternal;
        onWeaponFired += ClientUIController.instance.UpdateBulletNumberText;
    }

    // Invoked by external objects
    public virtual void Fire()
    {
        onWeaponFired?.Invoke(currentNumberOfBullets, bulletPerMagazine);
    }

    // Fire the weapon
    protected virtual void FireInternal(int arg1, int arg2) 
    {
        Debug.Log("Fire!");
        currentNumberOfBullets--;
    }

    [ServerRpc]
    protected virtual void SpawnBulletServerRpc(Quaternion facingDiretion) { }

    public void Reload()
    {
        currentNumberOfBullets = bulletPerMagazine;
    }

    protected virtual void Update()
    {
        timer += Time.deltaTime;
    }
}
