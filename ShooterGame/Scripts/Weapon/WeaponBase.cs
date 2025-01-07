using System.Collections;
using UnityEngine;

public class WeaponBase : MonoBehaviour
{
    public int weaponDamage;

    [Header("Bullet Settings")]
    public GameObject bulletPrefab;
    public Transform bulletSpawn;
    public float bulletVelocity = 30f;
    public float bulletPrefabLifetime = 3f;
    public float fireRate = 0.5f;
    protected float lastShotTime;
    protected float spawnOffset = 1.5f; // so that I don't shoot myself with ADS

    protected virtual void Start()
    {
        if (bulletPrefab == null)
        {
            bulletPrefab = Resources.Load("Prefabs/BulletShell") as GameObject;
        }
    }

    protected virtual bool CanShoot()
    {
        return Time.time - lastShotTime >= fireRate;
    }

    protected void Fire(Vector3 targetDirection, float accuracy = 0f)
    {
        if (!CanShoot()) return;

        if (accuracy != 0f)
        {
            targetDirection = Quaternion.AngleAxis(Random.Range(-accuracy, accuracy), Vector3.up) * targetDirection;
        }

        var bulletRotation = Quaternion.LookRotation(targetDirection);
        var bulletSpawnPosition = bulletSpawn.position + (targetDirection * -spawnOffset);
        var bullet = Instantiate(bulletPrefab, bulletSpawnPosition, bulletRotation);

        var bulletRb = bullet.GetComponent<Rigidbody>();
        bulletRb.velocity = targetDirection * bulletVelocity;

        StartCoroutine(DestroyBulletAfterTime(bullet, bulletPrefabLifetime));
        lastShotTime = Time.time;
    }

    protected IEnumerator DestroyBulletAfterTime(GameObject bullet, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (bullet != null)
        {
            Destroy(bullet);
        }
    }
}
