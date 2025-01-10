using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class XBotEnemy : Enemy
{
    public XBotHand xBotHand;
    public float xBotDamage;

    void Start()
    {
        xBotHand = GetComponentInChildren<XBotHand>();
        if (xBotHand != null)
        {
            xBotHand.damage = xBotDamage;
        }
        agent = GetComponent<NavMeshAgent>();
    }

    public override void TakeDamage(int amount)
    {
        HP -= amount;
        if (HP <= 0)
        {
            isDead = true;
            var randomValue = Random.Range(0, 3);

            if (randomValue == 0)
            {
                animator.SetTrigger("DIE1");
            }
            else if (randomValue == 1)
            {
                animator.SetTrigger("DIE2");
            }
            else if (randomValue == 2)
            {
                animator.SetTrigger("DIE3");
            }

            StartCoroutine(DisableCollider());
            StartCoroutine(DisableAnimator());
            StartCoroutine(DisableGameObject());
        }
        else
        {
            animator.SetTrigger("DAMAGE");
        }
    }

    private IEnumerator DisableCollider()
    {
        yield return new WaitForSeconds(1f);
        gameObject.GetComponent<Collider>().enabled = false;
    }

    private IEnumerator DisableAnimator()
    {
        yield return new WaitForSeconds(5f);
        animator.enabled = false;
    }
    private IEnumerator DisableGameObject()
    {
        yield return new WaitForSeconds(10f);
        gameObject.SetActive(false);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 3.5f);  // attacking // stop attacking

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, 18f);  // start chasing

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 21f);  // stop chasing
    }
}
