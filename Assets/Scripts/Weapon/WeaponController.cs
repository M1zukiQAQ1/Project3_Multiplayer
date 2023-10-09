using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    public int totalMagazine;

    protected float timer;

    private void Start()
    { 
        transform.localPosition = Vector3.zero;
        transform.localRotation = new Quaternion(0, 0, 0, 0);
    }

    public virtual void Fire() { }

    [ServerRpc]
    protected virtual void SpawnBulletServerRpc(Quaternion facingDiretion) { }

    public void Reload()
    {
        currentNumberOfBullets = bulletPerMagazine;
        totalMagazine--;
    }

    public void RefillMagazines(int magazines)
    {
        totalMagazine += magazines;
    }

    protected virtual void Update()
    {
        timer += Time.deltaTime;
    }
}
