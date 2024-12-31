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

    [Header("Shooting")]
    public bool isShooting, readyToShoot;
    private bool allowReset = true;
    public float shootingDelay = 2f;

    [Header("Burst")]
    public int bulletsPerBurst = 3;
    public int burstBulletsLeft;

    [Header("Spread")]
    public float hipSpreadIntensity;
    public float adsSpreadIntensity;

    public GameObject muzzleEffect;
    internal Animator animator;

    [Header("Reloading")]
    public float reloadTime;
    public int magazineSize, bulletsLeft;
    public bool isReloading;

    public Vector3 spawnPosition;
    public Vector3 spawnRotation;
    public ShootingMode currentShootingMode;
    public WeaponModel weaponModel;

    private bool isADS;
    private float spreadIntensity;

    public void Awake()
    {
        readyToShoot = true;
        burstBulletsLeft = bulletsPerBurst;
        animator = GetComponent<Animator>();

        bulletsLeft = magazineSize;

        spreadIntensity = hipSpreadIntensity;
    }

    private void DelayedFire()
    {
        bulletsLeft--;
        muzzleEffect.GetComponent<ParticleSystem>().Play();

        if (isADS)
        {
            animator.SetTrigger("RECOIL_ADS");
        }
        else
        {

            animator.SetTrigger("RECOIL");
        }

        SoundManager.Instance.PlayShootingSound(weaponModel);
        readyToShoot = false;

        var shootingDirection = CalculateDirectionAndSpread().normalized;
        var bulletRotation = Quaternion.LookRotation(shootingDirection);
        var bulletSpawnPosition = bulletSpawn.position + (shootingDirection * spawnOffset);
        var bullet = Instantiate(bulletPrefab, bulletSpawnPosition, bulletRotation);

        bullet.transform.forward = shootingDirection;
        bullet.GetComponent<Rigidbody>().velocity = shootingDirection * bulletVelocity;

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
        var bulletsNeeded = magazineSize - bulletsLeft;
        var availableAmmo = WeaponManager.Instance.CheckAmmoLeft(weaponModel);
        var bulletsToReload = Mathf.Min(bulletsNeeded, availableAmmo);

        bulletsLeft += bulletsToReload;

        WeaponManager.Instance.DecreaseTotalAmmo(bulletsToReload, weaponModel);

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
        var z = Random.Range(-spreadIntensity, spreadIntensity);
        var y = Random.Range(-spreadIntensity, spreadIntensity);

        return direction + new Vector3(0, y, z);
    }

    // TODO: Will I ever use this?
    private void RapidFire()
    {
        // Fire with no restrictions
        muzzleEffect.GetComponent<ParticleSystem>().Play();

        if (isADS)
        {
            animator.SetTrigger("RECOIL_ADS");
        }
        else
        {

            animator.SetTrigger("RECOIL");
        }

        SoundManager.Instance.PlayShootingSound(weaponModel);

        var bullet = Instantiate(bulletPrefab, bulletSpawn.position, Quaternion.identity);
        bullet.GetComponent<Rigidbody>().AddForce(bulletSpawn.forward.normalized * bulletVelocity, ForceMode.Impulse);
        StartCoroutine(DestroyBulletAfterTime(bullet, bulletPrefabLifetime));
    }

    void Update()
    {
        if (isActiveWeapon)
        {
            // Right mouse button for ADS
            if (Input.GetMouseButtonDown(1))
            {
                EnterADS();
            }
            if (Input.GetMouseButtonUp(1))
            {
                ExitADS();
            }

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

            if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < magazineSize && isReloading == false && WeaponManager.Instance.CheckAmmoLeft(weaponModel) > 0)
            {
                Reload();
            }

            // automatic reload when the magazine is empty
            if (readyToShoot && !isShooting && !isReloading && bulletsLeft <= 0)
            {
                // Reload();
            }

            if (readyToShoot && isShooting && bulletsLeft > 0)
            {
                burstBulletsLeft = bulletsPerBurst;
                DelayedFire();
            }
        }
    }

    private void EnterADS()
    {
        animator.SetTrigger("enterADS");
        isADS = true;
        HUDManager.Instance.crosshair.SetActive(false);
        spreadIntensity = adsSpreadIntensity;
    }

    private void ExitADS()
    {
        animator.SetTrigger("exitADS");
        isADS = false;
        HUDManager.Instance.crosshair.SetActive(true);
        spreadIntensity = hipSpreadIntensity;
    }

}