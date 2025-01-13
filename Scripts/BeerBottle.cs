using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeerBottle : MonoBehaviour
{
    public List<Rigidbody> parts = new List<Rigidbody>();

    public void Shatter()
    {
        foreach (var part in parts)
        {
            part.isKinematic = false;
        }
    }
}
