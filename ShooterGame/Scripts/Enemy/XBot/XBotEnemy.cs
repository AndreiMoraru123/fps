
using UnityEngine;

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
