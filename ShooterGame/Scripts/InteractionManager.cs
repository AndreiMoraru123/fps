using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    public static InteractionManager Instance { get; set; }
    public PlayerWeapon hoveredWeapon = null;
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
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            var objectHit = hit.transform.gameObject;
            if (objectHit.GetComponent<PlayerWeapon>() && objectHit.GetComponent<PlayerWeapon>().isActiveWeapon == false)
            {
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
        }
        else
        {
            if (hoveredWeapon)
            {
                hoveredWeapon.GetComponent<Outline>().enabled = false;
                hoveredWeapon = null;
            }
        }
    }
}
