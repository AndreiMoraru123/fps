using System;
using UnityEngine;

public enum ThrowableType
{
    Grenade
}

public class Throwable : MonoBehaviour
{
    [SerializeField]
    private float delay = 3f;

    [SerializeField]
    private float damageRadius = 20f;

    [SerializeField]
    private float explosionForce = 1200f;

    private float countdown;
    private bool hasExploded = false;
    public bool hasBeenThrown = false;
    public ThrowableType throwableType;

    private void Start()
    {
        countdown = delay;
    }

    private void Update()
    {
        if (hasBeenThrown)
        {
            countdown -= Time.deltaTime;
            if (countdown <= 0f && !hasExploded)
            {
                Explode();
                hasExploded = true;
            }
        }
    }

    private void Explode()
    {
        GetThrowableEffect();
        Destroy(gameObject);
    }

    private void GetThrowableEffect()
    {
        switch (throwableType)
        {
            case ThrowableType.Grenade:
                GrenadeEffect();
                break;
        }
    }

    private void GrenadeEffect()
    {
        // Visual Effect
        var explosionEffect = GlobalReferences.Instance.grenadeExplosionEffect;
        Instantiate(explosionEffect, transform.position, transform.rotation);

        // Physical Effect
        var colliders = Physics.OverlapSphere(transform.position, damageRadius);
        foreach (var objectInRange in colliders)
        {
            var rb = objectInRange.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(explosionForce, transform.position, damageRadius);
            }

            // TODO: apply damage to enemy
        }
    }
}