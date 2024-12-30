using TMPro;
using UnityEngine;

public enum ShootingMode
{
    Single, Burst, Auto
}

public enum WeaponModel
{
    M1911,
    AK47
}

public class PlayerWeapon : WeaponBase
{
    public bool isActiveWeapon;

    public bool isShooting, readyToShoot;
    private bool allowReset = true;
    public float shootingDelay = 2f;
    public int bulletsPerBurst = 3;
    public int burstBulletsLeft;
    public float spreadIntensity;

    public GameObject muzzleEffect;
    internal Animator animator;

    // Reloading
    public float reloadTime;
    public int magazineSize, bulletsLeft;
    public bool isReloading;

    public Vector3 spawnPosition;
    public Vector3 spawnRotation;
    public ShootingMode currentShootingMode;
    public WeaponModel weaponModel;

    public void Awake()
    {
        readyToShoot = true;
        burstBulletsLeft = bulletsPerBurst;
        animator = GetComponent<Animator>();

        bulletsLeft = magazineSize;
    }

    private void DelayedFire()
    {
        bulletsLeft--;

        muzzleEffect.GetComponent<ParticleSystem>().Play();
        animator.SetTrigger("RECOIL");

        SoundManager.Instance.PlayShootingSound(weaponModel);

        readyToShoot = false;
        var shootingDirection = CalculateDirectionAndSpread().normalized;

        var bullet = Instantiate(bulletPrefab, bulletSpawn.position, Quaternion.identity);
        bullet.transform.forward = shootingDirection;

        bullet.GetComponent<Rigidbody>().AddForce(shootingDirection * bulletVelocity, ForceMode.Impulse);
        StartCoroutine(DestroyBulletAfterTime(bullet, bulletPrefabLifetime));

        if (allowReset)
        {
            Invoke("ResetShot", shootingDelay);
            allowReset = false;
        }

        // we already shoot once before this check
        if (currentShootingMode == ShootingMode.Burst && burstBulletsLeft > 1)
        {
            burstBulletsLeft--;
            Invoke("DelayedFire", shootingDelay);
        }
    }

    private void Reload()
    {

        SoundManager.Instance.PlayReloadSound(weaponModel);

        animator.SetTrigger("RELOAD");

        isReloading = true;
        Invoke("ReloadCompleted", reloadTime);
    }

    private void ReloadCompleted()
    {
        bulletsLeft = magazineSize;
        isReloading = false;
    }

    private void ResetShot()
    {
        readyToShoot = true;
        allowReset = true;
    }

    public Vector3 CalculateDirectionAndSpread()
    {
        var ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        Vector3 targetPoint;

        if (Physics.Raycast(ray, out hit))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = ray.GetPoint(100);
        }

        var direction = targetPoint - bulletSpawn.position;
        var x = Random.Range(-spreadIntensity, spreadIntensity);
        var y = Random.Range(-spreadIntensity, spreadIntensity);

        return direction + new Vector3(x, y, 0);
    }

    // TODO: Will I ever use this?
    private void RapidFire()
    {
        // Fire with no restrictions
        muzzleEffect.GetComponent<ParticleSystem>().Play();
        animator.SetTrigger("RECOIL");

        SoundManager.Instance.PlayShootingSound(weaponModel);

        var bullet = Instantiate(bulletPrefab, bulletSpawn.position, Quaternion.identity);
        bullet.GetComponent<Rigidbody>().AddForce(bulletSpawn.forward.normalized * bulletVelocity, ForceMode.Impulse);
        StartCoroutine(DestroyBulletAfterTime(bullet, bulletPrefabLifetime));
    }

    void Update()
    {
        if (isActiveWeapon)
        {
            GetComponent<Outline>().enabled = false;

            if (bulletsLeft == 0 && isShooting)
            {
                SoundManager.Instance.emptyMagazineSoundM1911.Play();
            }

            if (currentShootingMode == ShootingMode.Auto)
            {
                // hold the LMB
                isShooting = Input.GetKey(KeyCode.Mouse0);
            }
            else if (currentShootingMode == ShootingMode.Single || currentShootingMode == ShootingMode.Burst)
            {
                isShooting = Input.GetKeyDown(KeyCode.Mouse0);
            }

            if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < magazineSize && isReloading == false)
            {
                Reload();
            }

            // automatic reload when the magazine is empty
            if (readyToShoot && !isShooting && !isReloading && bulletsLeft <= 0)
            {
                Reload();
            }

            if (readyToShoot && isShooting && bulletsLeft > 0)
            {
                burstBulletsLeft = bulletsPerBurst;
                DelayedFire();
            }
        }
    }
}