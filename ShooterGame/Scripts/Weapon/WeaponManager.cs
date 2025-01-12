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
    public float throwForce = 10f;
    public GameObject throwableSpawn;
    public float forceMultiplier = 0f;
    public float forceMultiplierLimit = 2f;

    [Header("Lethals")]
    public int maxLethals = 2;
    public int lethalsCount = 0;
    public ThrowableType equippedLethalType;
    public GameObject grenadePrefab;

    [Header("Tacticals")]
    public int maxTacticals = 2;
    public int tacticalsCount = 0;
    public ThrowableType equippedTacticalType;
    public GameObject smokeGrenadePrefab;

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
        equippedLethalType = ThrowableType.None;
        equippedTacticalType = ThrowableType.None;
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

        if (Input.GetKey(KeyCode.G) || Input.GetKey(KeyCode.T))
        {
            forceMultiplier += Time.deltaTime;
            if (forceMultiplier > forceMultiplierLimit)
            {
                forceMultiplier = forceMultiplierLimit;
            }
        }

        if (Input.GetKeyUp(KeyCode.G))
        {
            if (lethalsCount > 0)
            {
                ThrowLethal();
            }

            forceMultiplier = 0f;
        }

        if (Input.GetKeyUp(KeyCode.T))
        {
            if (tacticalsCount > 0)
            {
                ThrowTactical();
            }

            forceMultiplier = 0f;
        }
    }

    private void ThrowTactical()
    {
        var tacticalPrefab = GetThrowablePrefab(equippedTacticalType);
        var throwable = Instantiate(tacticalPrefab, throwableSpawn.transform.position, Camera.main.transform.rotation);
        var rb = throwable.GetComponent<Rigidbody>();

        rb.AddForce(Camera.main.transform.forward * (throwForce * forceMultiplier), ForceMode.Impulse);
        throwable.GetComponent<Throwable>().hasBeenThrown = true;

        tacticalsCount--;
        if (tacticalsCount <= 0)
        {
            equippedTacticalType = ThrowableType.None;
        }

        HUDManager.Instance.UpdateThrowablesUI();
    }


    private void ThrowLethal()
    {
        var lethalPrefab = GetThrowablePrefab(equippedLethalType);
        var throwable = Instantiate(lethalPrefab, throwableSpawn.transform.position, Camera.main.transform.rotation);
        var rb = throwable.GetComponent<Rigidbody>();

        rb.AddForce(Camera.main.transform.forward * (throwForce * forceMultiplier), ForceMode.Impulse);
        throwable.GetComponent<Throwable>().hasBeenThrown = true;

        lethalsCount--;
        if (lethalsCount <= 0)
        {
            equippedLethalType = ThrowableType.None;
        }

        HUDManager.Instance.UpdateThrowablesUI();
    }

    private GameObject GetThrowablePrefab(ThrowableType throwableType)
    {
        switch (throwableType)
        {
            case ThrowableType.Grenade:
                return grenadePrefab;
            case ThrowableType.Smoke:
                return smokeGrenadePrefab;
        }

        return new();
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
        switch (throwable.throwableData.throwableType)
        {
            case ThrowableType.Grenade:
                PickUpThrowableAsLethal(ThrowableType.Grenade);
                break;
            case ThrowableType.Smoke:
                PickUpThrowableAsTactical(ThrowableType.Smoke);
                break;
        }
    }

    private void PickUpThrowableAsTactical(ThrowableType tactical)
    {
        if (equippedTacticalType == tactical || equippedTacticalType == ThrowableType.None)
        {
            equippedTacticalType = tactical;
            if (tacticalsCount < maxTacticals)
            {
                tacticalsCount++;
                Destroy(InteractionManager.Instance.hoveredThrowable.gameObject);
                HUDManager.Instance.UpdateThrowablesUI();
            }
            else
            {
                print("tacticals limit reached");
            }
        }
        else
        {
            // cannot pick up different tactical
        }
    }


    private void PickUpThrowableAsLethal(ThrowableType lethal)
    {
        if (equippedLethalType == lethal || equippedLethalType == ThrowableType.None)
        {
            equippedLethalType = lethal;
            if (lethalsCount < maxLethals)
            {
                lethalsCount++;
                Destroy(InteractionManager.Instance.hoveredThrowable.gameObject);
                HUDManager.Instance.UpdateThrowablesUI();
            }
            else
            {
                print("lethals limit reached");
            }
        }
        else
        {
            // cannot pick up different lethal
        }
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
