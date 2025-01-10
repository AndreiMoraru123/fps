
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class YBotEnemy : Enemy
{
    public Transform gunBarrel;
    private GameObject player;
    public GameObject Player { get => player; }

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    public override void TakeDamage(int amount)
    {
        HP -= amount;
        if (HP <= 0)
        {
            isDead = true;
            animator.SetTrigger("DIE");

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
        Gizmos.DrawWireSphere(transform.position, 12.5f);  // attacking // stop attacking

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, 18f);  // start chasing

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 21f);  // stop chasing
    }

}

