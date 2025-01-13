using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;

public class ThrowablePickUp : PickUp
{
    protected override void Interact()
    {
        var hoveredThrowable = InteractionManager.Instance.hoveredThrowable;
        if (hoveredThrowable != null)
        {
            WeaponManager.Instance.PickUpThrowable(hoveredThrowable);
            InteractionManager.Instance.hoveredThrowable = null;
        }
    }

}
