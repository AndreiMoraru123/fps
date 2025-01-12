using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    protected int HP = 100;
    protected Animator animator;
    public bool isDead;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public virtual void TakeDamage(int amount) { }

    public void ApplyBlindness()
    {
        animator.SetTrigger("BLINDED");
    }
    protected IEnumerator DisableCollider()
    {
        yield return new WaitForSeconds(1f);
        gameObject.GetComponent<Collider>().enabled = false;
    }
    protected IEnumerator DisableAnimator()
    {
        yield return new WaitForSeconds(5f);
        animator.enabled = false;
    }
    protected IEnumerator DisableGameObject()
    {
        yield return new WaitForSeconds(10f);
        gameObject.SetActive(false);
    }
}
