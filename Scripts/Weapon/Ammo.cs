using UnityEngine;

public enum AmmoType
{
    AK47Ammo,
    M1911Ammo
}

public class Ammo : MonoBehaviour
{
    public int ammoAmount = 200;
    public AmmoType ammoType;

}