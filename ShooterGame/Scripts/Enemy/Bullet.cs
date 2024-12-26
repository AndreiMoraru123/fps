using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        Transform hitTransform = collision.transform;
        if (hitTransform.CompareTag("Player"))
        {
            Debug.Log("Hit Player");
            hitTransform.GetComponent<PlayerHealth>().TakeDamage(10);
        }

        if (hitTransform.CompareTag("Target"))
        {
            Debug.Log("Hit " + hitTransform.gameObject.name);
            CreateBulletImpactEffect(collision);
            Destroy(gameObject);
        }

        if (hitTransform.CompareTag("Solid"))
        {
            Debug.Log("Hit a wall");
            CreateBulletImpactEffect(collision);
            Destroy(gameObject);
        }

        if (hitTransform.CompareTag("Beer"))
        {
            Debug.Log("Hit a wall");
            collision.gameObject.GetComponent<BeerBottle>().Shatter();
        }
    }

    private void CreateBulletImpactEffect(Collision collision)
    {
        ContactPoint contact = collision.contacts[0];
        var hole = Instantiate(GlobalReferences.Instance.bulletImpactEffectPrefab, contact.point, Quaternion.LookRotation(contact.normal));
        hole.transform.SetParent(collision.gameObject.transform);
    }
}
