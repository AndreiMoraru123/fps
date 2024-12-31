using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;

public class AmmoPickUp : PickUp
{
    protected override void Interact()
    {
        var hoveredAmmo = InteractionManager.Instance.hoveredAmmo;
        if (hoveredAmmo != null)
        {
            WeaponManager.Instance.PickupAmmo(hoveredAmmo);
            InteractionManager.Instance.hoveredAmmo = null;
        }
    }

}
