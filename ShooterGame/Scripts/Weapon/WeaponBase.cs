using System.Collections;
using UnityEngine;

public class WeaponBase : MonoBehaviour
{
    public int weaponDamage;

    [Header("Bullet Settings")]
    public GameObject bulletPrefab;
    public Transform bulletSpawn;
    public float bulletVelocity = 300f;
    public float bulletPrefabLifetime = 3f;
    protected float lastShotTime;
    protected float bulletSpawnOffset = 2.5f;  // so that I don't shoot myself

    protected virtual void Start()
    {
        if (bulletPrefab == null)
        {
            bulletPrefab = Resources.Load("Prefabs/BulletShell") as GameObject;
        }
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
