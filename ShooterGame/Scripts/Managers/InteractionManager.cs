using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    public static InteractionManager Instance { get; set; }
    public PlayerWeapon hoveredWeapon = null;
    public Ammo hoveredAmmo = null;
    public Throwable hoveredThrowable = null;
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

    public void Update()
    {
        var ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            var objectHit = hit.transform.gameObject;
            if (objectHit.GetComponent<PlayerWeapon>() && objectHit.GetComponent<PlayerWeapon>().isActiveWeapon == false)
            {
                // disable the outline of previously selected item
                if (hoveredWeapon)
                {
                    hoveredWeapon.GetComponent<Outline>().enabled = false;
                }

                hoveredWeapon = objectHit.gameObject.GetComponent<PlayerWeapon>();
                var pickUp = objectHit.GetComponent<WeaponPickUp>();

                hoveredWeapon.GetComponent<Outline>().enabled = true;

                if (pickUp != null && pickUp.ValidateInteraction())
                {
                    pickUp.BaseInteract();
                }
            }
            else
            {
                if (hoveredWeapon)
                {
                    hoveredWeapon.GetComponent<Outline>().enabled = false;
                    hoveredWeapon = null;
                }
            }

            // Ammo
            if (objectHit.GetComponent<Ammo>())
            {
                // disable the outline of previously selected item
                if (hoveredAmmo)
                {
                    hoveredAmmo.GetComponent<Outline>().enabled = false;
                }

                hoveredAmmo = objectHit.gameObject.GetComponent<Ammo>();
                var pickUp = objectHit.GetComponent<AmmoPickUp>();

                hoveredAmmo.GetComponent<Outline>().enabled = true;

                if (pickUp != null && pickUp.ValidateInteraction())
                {
                    pickUp.BaseInteract();
                    Destroy(objectHit.gameObject);
                }
            }
            else
            {
                if (hoveredAmmo)
                {
                    hoveredAmmo.GetComponent<Outline>().enabled = false;
                    hoveredAmmo = null;
                }
            }

            // Throwable
            if (objectHit.GetComponent<Throwable>())
            {
                if (hoveredThrowable)
                {
                    hoveredThrowable.GetComponent<Outline>().enabled = false;
                }

                hoveredThrowable = objectHit.gameObject.GetComponent<Throwable>();
                var pickUp = objectHit.GetComponent<ThrowablePickUp>();

                hoveredThrowable.GetComponent<Outline>().enabled = true;

                if (pickUp != null && pickUp.ValidateInteraction())
                {
                    pickUp.BaseInteract();
                }
            }
            else
            {
                if (hoveredThrowable)
                {
                    hoveredThrowable.GetComponent<Outline>().enabled = false;
                    hoveredThrowable = null;
                }
            }
        }
    }
}
