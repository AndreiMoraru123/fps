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
    public int magazineSize;
    public int bulletsLeft;
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
            spawnOffset = 2.5f;
            animator.SetTrigger("RECOIL_ADS");
        }
        else
        {
            spawnOffset = 1.5f;
            animator.SetTrigger("RECOIL");
        }

        SoundManager.Instance.PlayShootingSound(weaponModel);
        readyToShoot = false;

        var shootingDirection = CalculateDirectionAndSpread().normalized;
        var bulletRotation = Quaternion.LookRotation(shootingDirection);
        var bulletSpawnPosition = bulletSpawn.position + (shootingDirection * spawnOffset);
        var bullet = Instantiate(bulletPrefab, bulletSpawnPosition, bulletRotation);

        var bul = bullet.GetComponent<Bullet>();
        bul.bulletDamage = weaponDamage;

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
        var ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)); // center of screen
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
        var spread = spreadIntensity * Mathf.Deg2Rad;
        var theta = Random.Range(0f, 2f * Mathf.PI); // spin like a clock
        var phi = Random.Range(0f, spread); // how far from the center of the bullseye
        var aimNormal = Vector3.Cross(direction, Vector3.up); // perpendicular to the aim direction
        var randomRotation = Quaternion.AngleAxis(theta * Mathf.Rad2Deg, direction) *
                                        Quaternion.AngleAxis(phi * Mathf.Rad2Deg, aimNormal);

        return randomRotation * direction;
    }

    // TODO: Will I ever use this?
    private void RapidFire()
    {
        // Fire with no restrictions
        muzzleEffect.GetComponent<ParticleSystem>().Play();

        if (isADS)
        {
            spawnOffset = 2.5f;
            animator.SetTrigger("RECOIL_ADS");
        }
        else
        {
            spawnOffset = 0.5f;
            animator.SetTrigger("RECOIL");
        }

        SoundManager.Instance.PlayShootingSound(weaponModel);

        var bullet = Instantiate(bulletPrefab, bulletSpawn.position, Quaternion.identity);
        bullet.GetComponent<Rigidbody>().AddForce(bulletSpawn.forward.normalized * bulletVelocity, ForceMode.Impulse);

        var bul = bullet.GetComponent<Bullet>();
        bul.bulletDamage = weaponDamage;

        StartCoroutine(DestroyBulletAfterTime(bullet, bulletPrefabLifetime));
    }


    void Update()
    {
        if (isActiveWeapon)
        {
            // Avoid clipping through walls
            SetLayerRecursively(gameObject, LayerMask.NameToLayer("WeaponRender"));

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

            if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < magazineSize && !isReloading && WeaponManager.Instance.CheckAmmoLeft(weaponModel) > 0)
            {
                Reload();
            }

            // [Disabled] automatic reload when the magazine is empty
            if (readyToShoot && !isShooting && !isReloading && bulletsLeft <= 0 && WeaponManager.Instance.CheckAmmoLeft(weaponModel) > 0)
            {
                // Reload();
            }

            if (readyToShoot && isShooting && bulletsLeft > 0)
            {
                burstBulletsLeft = bulletsPerBurst;
                DelayedFire();
            }
        }
        else
        {
            // back to picking up layer
            SetLayerRecursively(gameObject, LayerMask.NameToLayer("Weapon"));
        }
    }

    private void SetLayerRecursively(GameObject obj, int newLayer)
    {
        obj.layer = newLayer;

        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, newLayer);
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