
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapon : WeaponBase
{
    protected override void Start()
    {
        base.Start();
        bulletVelocity = 50f;
        fireRate = 0.1f;
    }
    public void RapidFire()
    {
        // TODO: fire with no restrictions
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, Quaternion.identity);
        bullet.GetComponent<Rigidbody>().AddForce(bulletSpawn.forward.normalized * bulletVelocity, ForceMode.Impulse);
        StartCoroutine(DestroyBulletAfterTime(bullet, bulletPrefabLifetime));
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Fire(bulletSpawn.forward);
        }
    }
}