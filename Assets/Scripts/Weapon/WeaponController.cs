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
    [SerializeField] protected int reloadingTime;
    [SerializeField] protected int currentNumberOfBullets;
    
    protected float timer;
    
    // currentBullets, bulletsInOneMagazine
    protected event Action<int, int> onWeaponFired;
    protected event Action<int, int> onWeaponReloaded;

    [SerializeField] private bool isEnemiesWeapon = false;

    private void Start()
    {
        isEnemiesWeapon = owner.GetComponent<EnemyController>() != null;

        transform.localPosition = Vector3.zero;
        transform.localRotation = new Quaternion(0, 0, 0, 0);

        onWeaponFired += FireInternal;
        if(!isEnemiesWeapon)
            onWeaponFired += ClientUIController.instance.UpdateBulletNumberText;

        onWeaponReloaded += Reload;
    }

    // Designed to be invoke by external objects
    // Returns true if there're bullets left in the magazine
    public bool Fire()
    {
        Debug.Log($"WeaponController: onWeaponFired invoke, {currentNumberOfBullets} / {bulletPerMagazine}");
        if (currentNumberOfBullets > 0 && timer >= (60 / firePerMinute))
        {
            onWeaponFired?.Invoke(--currentNumberOfBullets, bulletPerMagazine);
            return true;
        }
        return currentNumberOfBullets > 0;
    }

    // Fire the weapon
    protected virtual void FireInternal(int arg1, int arg2) 
    {
        Debug.Log("Fire!");
    }

    [ServerRpc]
    protected virtual void SpawnBulletServerRpc(Quaternion facingDiretion) { }

    public void Reload()
    {
        onWeaponReloaded?.Invoke(currentNumberOfBullets, bulletPerMagazine);
    }

    //Bug: If pressed when reloading, it will still execute
    protected void Reload(int arg1, int arg2)
    {
        StartCoroutine(IEReload());
    }

    private IEnumerator IEReload()
    {
        if (isEnemiesWeapon)
        {
            yield return new WaitForSeconds(reloadingTime);
            currentNumberOfBullets = bulletPerMagazine;
            yield break;
        }

        ClientUIController.instance.UpdateBulletNumberText("Reloading");
        yield return new WaitForSeconds(reloadingTime);
        currentNumberOfBullets = bulletPerMagazine;
        ClientUIController.instance.UpdateBulletNumberText(currentNumberOfBullets, bulletPerMagazine);
    }

    protected virtual void Update()
    {
        timer += Time.deltaTime;
    }
}
