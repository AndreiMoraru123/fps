using System;
using UnityEngine;

public enum ThrowableType
{
    None,
    Grenade,
    Smoke
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
            case ThrowableType.Smoke:
                SmokeEffect();
                break;
        }
    }

    private void SmokeEffect()
    {
        // Visual Effect
        var smokeEffect = GlobalReferences.Instance.smokeGrenadeEffect;
        Instantiate(smokeEffect, transform.position, transform.rotation);

        // Sound Effect
        SoundManager.Instance.throwablesChannel.PlayOneShot(SoundManager.Instance.grenadeSound);

        // Physical Effect
        var colliders = Physics.OverlapSphere(transform.position, damageRadius);
        foreach (var objectInRange in colliders)
        {
            var rb = objectInRange.GetComponent<Rigidbody>();
            if (rb != null)
            {
                // TODO: apply blindness to enemies
            }

        }
    }

    private void GrenadeEffect()
    {
        // Visual Effect
        var explosionEffect = GlobalReferences.Instance.grenadeExplosionEffect;
        Instantiate(explosionEffect, transform.position, transform.rotation);

        // Sound Effect
        SoundManager.Instance.throwablesChannel.PlayOneShot(SoundManager.Instance.grenadeSound);

        // Physical Effect
        var colliders = Physics.OverlapSphere(transform.position, damageRadius);
        foreach (var objectInRange in colliders)
        {
            var directionToTarget = (objectInRange.transform.position - transform.position).normalized;
            var distanceToTarget = Vector3.Distance(transform.position, objectInRange.transform.position);

            RaycastHit hit;
            if (Physics.Raycast(transform.position, directionToTarget, out hit, distanceToTarget))
            {
                // the explosion cannot go through walls
                if (hit.collider.CompareTag("Solid") && hit.collider != objectInRange)
                {
                    continue;
                }

                // the explosion will damage the Player
                if (hit.collider.CompareTag("Player"))
                {
                    var player = hit.collider.GetComponent<PlayerHealth>();
                    if (player.isDead == false)
                    {
                        player.TakeDamage(100);
                    }
                }

                // the explosion will shatter bottles
                if (hit.collider.CompareTag("Beer"))
                {
                    hit.collider.gameObject.GetComponent<BeerBottle>().Shatter();
                }
            }

            // add explosion force and damage only if there is a line of sight
            var rb = objectInRange.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(explosionForce, transform.position, damageRadius);
            }

            if (objectInRange.gameObject.TryGetComponent(out Enemy enemy) && !enemy.isDead)
            {
                // TODO: Do I want to hard code this?
                enemy.TakeDamage(100);
            }
        }
    }
}