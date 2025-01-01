using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    protected int HP = 100;
    protected Animator animator;
    protected NavMeshAgent agent;
    public NavMeshAgent Agent { get => agent; }

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public virtual void TakeDamage(int amount)
    {
        HP -= amount;
        if (HP <= 0)
        {
            animator.SetTrigger("DIE");
            Destroy(gameObject);
        }
        else
        {
            animator.SetTrigger("DAMAGE");
        }
    }
}
