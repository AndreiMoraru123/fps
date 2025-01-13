
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;

public class WeaponPickUp : PickUp
{
    protected override void Interact()
    {
        var hoveredWeapon = InteractionManager.Instance.hoveredWeapon;
        if (hoveredWeapon != null)
        {
            WeaponManager.Instance.PickupWeapon(hoveredWeapon.gameObject);
            InteractionManager.Instance.hoveredWeapon = null;
        }
    }

}
