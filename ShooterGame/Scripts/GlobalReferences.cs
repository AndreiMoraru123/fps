using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalReferences : MonoBehaviour
{
    // Singleton pattern
    public static GlobalReferences Instance { get; set; }

    public GameObject bulletImpactEffectPrefab;
    public GameObject grenadeExplosionEffect;

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
}
