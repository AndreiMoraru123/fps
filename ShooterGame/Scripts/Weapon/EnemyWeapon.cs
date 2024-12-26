using UnityEngine;

public class EnemyWeapon : WeaponBase
{
    [Header("Enemy Weapon Settings")]
    public float accuracy = 3f;

    protected override void Start()
    {
        base.Start();
        bulletVelocity = 40f;
    }
    public void ShootAtTarget(Transform target)
    {
        if (!CanShoot()) return;
        Vector3 shootDirection = (target.position - bulletSpawn.position).normalized;
        Fire(shootDirection, accuracy);
    }
}