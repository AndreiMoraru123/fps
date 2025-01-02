using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    protected int HP = 100;

    // TODO: do I use this getter?
    public int GetHealth { get => HP; }
    protected Animator animator;
    protected NavMeshAgent agent;
    public NavMeshAgent Agent { get => agent; }
    public bool isDead;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public virtual void TakeDamage(int amount) { }
}
