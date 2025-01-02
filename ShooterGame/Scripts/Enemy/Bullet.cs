using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int bulletDamage;
    private void OnCollisionEnter(Collision collision)
    {
        var hitTransform = collision.transform;

        if (hitTransform.CompareTag("Player"))
        {
            hitTransform.GetComponent<PlayerHealth>().TakeDamage(10);
        }

        if (hitTransform.CompareTag("Target"))
        {
            CreateBulletImpactEffect(collision);
            Destroy(gameObject);
        }

        if (hitTransform.CompareTag("Solid"))
        {
            CreateBulletImpactEffect(collision);
            Destroy(gameObject);
        }

        if (hitTransform.CompareTag("Beer"))
        {
            collision.gameObject.GetComponent<BeerBottle>().Shatter();
        }

        if (hitTransform.CompareTag("Enemy"))
        {
            var enemy = collision.gameObject.GetComponent<Enemy>();
            if (enemy.isDead == false)
            {
                enemy.TakeDamage(bulletDamage);
            }

            CreateBloodSprayEffect(collision);
            Destroy(gameObject);
        }
    }

    private void CreateBloodSprayEffect(Collision collision)
    {
        var contact = collision.contacts[0];
        var blood = Instantiate(GlobalReferences.Instance.bloodSprayEffect, contact.point, Quaternion.LookRotation(contact.normal));
        blood.transform.SetParent(collision.gameObject.transform);
    }


    private void CreateBulletImpactEffect(Collision collision)
    {
        var contact = collision.contacts[0];
        var hole = Instantiate(GlobalReferences.Instance.bulletImpactEffectPrefab, contact.point, Quaternion.LookRotation(contact.normal));
        hole.transform.SetParent(collision.gameObject.transform);
    }
}
