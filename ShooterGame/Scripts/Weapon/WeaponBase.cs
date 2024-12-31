using System.Collections;
using UnityEngine;

public class WeaponBase : MonoBehaviour
{
    [Header("Bullet Settings")]
    public GameObject bulletPrefab;
    public Transform bulletSpawn;
    public float bulletVelocity = 30f;
    public float bulletPrefabLifetime = 3f;
    public float fireRate = 0.5f;
    protected float lastShotTime;

    protected virtual void Start()
    {
        if (bulletPrefab == null)
        {
            bulletPrefab = Resources.Load("Prefabs/Bullet") as GameObject;
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
        var bullet = Instantiate(bulletPrefab, bulletSpawn.position, bulletRotation);

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
