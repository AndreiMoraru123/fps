using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public static WeaponManager Instance { get; set; }
    public List<GameObject> weaponSlots;
    public GameObject activeWeaponSlot;

    [Header("Ammo")]
    public int totalM1911Ammo = 0;
    public int totalAK47Ammo = 0;

    [Header("Throwables")]
    public int grenades = 0;
    public float throwForce = 10f;
    public GameObject grenadePrefab;
    public GameObject throwableSpawn;
    public float forceMultiplier = 0f;
    public float forceMultiplierLimit = 2f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public void Start()
    {
        activeWeaponSlot = weaponSlots[0];
    }

    public void Update()
    {
        foreach (var slot in weaponSlots)
        {
            if (slot == activeWeaponSlot)
            {
                slot.SetActive(true);
            }
            else
            {
                slot.SetActive(false);
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SwitchActiveSlot(0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SwitchActiveSlot(1);
        }

        if (Input.GetKey(KeyCode.G))
        {
            forceMultiplier += Time.deltaTime;
            if (forceMultiplier > forceMultiplierLimit)
            {
                forceMultiplier = forceMultiplierLimit;
            }
        }

        if (Input.GetKeyUp(KeyCode.G))
        {
            if (grenades > 0)
            {
                ThrowLethal();
            }

            forceMultiplier = 0f;
        }
    }

    private void ThrowLethal()
    {
        var lethalPrefab = grenadePrefab;
        var throwable = Instantiate(lethalPrefab, throwableSpawn.transform.position, Camera.main.transform.rotation);
        var rb = throwable.GetComponent<Rigidbody>();

        rb.AddForce(Camera.main.transform.forward * (throwForce * forceMultiplier), ForceMode.Impulse);
        throwable.GetComponent<Throwable>().hasBeenThrown = true;

        grenades--;
        HUDManager.Instance.UpdateThrowables(ThrowableType.Grenade);
    }


    public void PickupWeapon(GameObject weapon)
    {
        AddWeaponIntoActiveSlot(weapon);
    }

    public void PickupAmmo(Ammo ammo)
    {
        switch (ammo.ammoType)
        {
            case AmmoType.M1911Ammo:
                totalM1911Ammo += ammo.ammoAmount;
                break;
            case AmmoType.AK47Ammo:
                totalAK47Ammo += ammo.ammoAmount;
                break;
        }
    }

    public void PickUpThrowable(Throwable throwable)
    {
        switch (throwable.throwableType)
        {
            case ThrowableType.Grenade:
                PickUpGrenade();
                break;
        }
    }

    private void PickUpGrenade()
    {
        grenades++;
        HUDManager.Instance.UpdateThrowables(ThrowableType.Grenade);
    }


    private void AddWeaponIntoActiveSlot(GameObject pickedUpWeapon)
    {
        DropCurrentWeapon(pickedUpWeapon);

        pickedUpWeapon.transform.SetParent(activeWeaponSlot.transform, false);
        var weapon = pickedUpWeapon.GetComponent<PlayerWeapon>();

        pickedUpWeapon.transform.localPosition = new Vector3(weapon.spawnPosition.x, weapon.spawnPosition.y, weapon.spawnPosition.z);
        pickedUpWeapon.transform.localRotation = Quaternion.Euler(weapon.spawnRotation.x, weapon.spawnRotation.y, weapon.spawnRotation.z);

        weapon.isActiveWeapon = true;
        weapon.animator.enabled = true;
    }

    private void DropCurrentWeapon(GameObject pickedUpWeapon)
    {
        if (activeWeaponSlot.transform.childCount > 0)
        {
            var weaponToDrop = activeWeaponSlot.transform.GetChild(0).gameObject;
            var droppedWeapon = weaponToDrop.GetComponent<PlayerWeapon>();

            droppedWeapon.isActiveWeapon = false;
            droppedWeapon.animator.enabled = false;

            weaponToDrop.transform.SetParent(pickedUpWeapon.transform.parent);
            weaponToDrop.transform.localPosition = pickedUpWeapon.transform.localPosition;
            weaponToDrop.transform.localRotation = pickedUpWeapon.transform.localRotation;
        }
    }

    public void SwitchActiveSlot(int slotNumber)
    {
        if (activeWeaponSlot.transform.childCount > 0)
        {
            var currentWeapon = activeWeaponSlot.transform.GetChild(0).GetComponent<PlayerWeapon>();
            currentWeapon.isActiveWeapon = false;
        }

        activeWeaponSlot = weaponSlots[slotNumber];

        if (activeWeaponSlot.transform.childCount > 0)
        {
            var newWeapon = activeWeaponSlot.transform.GetChild(0).GetComponent<PlayerWeapon>();
            newWeapon.isActiveWeapon = true;
        }
    }

    internal void DecreaseTotalAmmo(int bulletsToDecrease, WeaponModel weaponModel)
    {
        switch (weaponModel)
        {
            case WeaponModel.M1911:
                totalM1911Ammo -= bulletsToDecrease;
                break;
            case WeaponModel.AK47:
                totalAK47Ammo -= bulletsToDecrease;
                break;
        }
    }

    public int CheckAmmoLeft(WeaponModel weaponModel)
    {
        return weaponModel switch
        {
            WeaponModel.M1911 => totalM1911Ammo,
            WeaponModel.AK47 => totalAK47Ammo,
            _ => 0,
        };
    }
}
