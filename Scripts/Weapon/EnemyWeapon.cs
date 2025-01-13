using UnityEngine;

public class EnemyWeapon : WeaponBase
{
    [Range(0f, 10f)]
    public float fireRate = 8.5f;

    public void ShootAtTarget(Transform target)
    {
        if (!CanShoot()) return;

        var shootingDirection = (target.position - bulletSpawn.position).normalized;
        var bulletRotation = Quaternion.LookRotation(shootingDirection);
        var bulletSpawnPosition = bulletSpawn.position + (shootingDirection * bulletSpawnOffset);
        var bullet = Instantiate(bulletPrefab, bulletSpawnPosition, bulletRotation);

        bullet.GetComponent<Bullet>().bulletDamage = weaponDamage;
        bullet.GetComponent<Rigidbody>().velocity = shootingDirection * bulletVelocity;

        StartCoroutine(DestroyBulletAfterTime(bullet, bulletPrefabLifetime));
    }
    protected virtual bool CanShoot()
    {
        return Time.time - lastShotTime >= (1 / fireRate);
    }


}