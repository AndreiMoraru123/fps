using System.Collections;
using UnityEngine;

public class WeaponBase : MonoBehaviour
{
    [Header("Weapon Settings")]
    public int weaponDamage;
    public float shootingDelay = 2f;

    [Header("Bullet Settings")]
    public GameObject bulletPrefab;
    public Transform bulletSpawn;
    public float bulletVelocity = 300f;
    public float bulletPrefabLifetime = 3f;
    protected float bulletSpawnOffset = 2.5f;  // so that I don't shoot myself
    protected bool readyToShoot;
    protected bool allowReset = true;

    protected virtual void Start()
    {
        if (bulletPrefab == null)
        {
            bulletPrefab = Resources.Load("Prefabs/BulletShell") as GameObject;
        }
    }
    protected void ResetShot()
    {
        readyToShoot = true;
        allowReset = true;
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
