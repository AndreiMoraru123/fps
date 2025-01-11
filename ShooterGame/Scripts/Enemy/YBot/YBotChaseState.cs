
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class YBotChaseState : StateMachineBehaviour
{
    NavMeshAgent agent;
    Transform player;
    YBotEnemy enemy;
    public float chaseSpeed = 6f;
    public float stopChasingDistance = 21f;
    public float attackingDistance = 12.5f;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        enemy = animator.GetComponent<YBotEnemy>();
        agent = animator.GetComponent<NavMeshAgent>();
        agent.speed = chaseSpeed;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var distanceFromPlayer = Vector3.Distance(player.position, animator.transform.position);

        // check if the agent should stop chasing (too far or can't see the player)
        if (distanceFromPlayer > stopChasingDistance || !enemy.CanSeeTarget(player))
        {
            animator.SetBool("isChasing", false);
            return;
        }

        // if it got here, it can see the player
        agent.SetDestination(player.position);
        animator.transform.LookAt(player);

        // check if the agent should be attacking
        if (distanceFromPlayer < attackingDistance)
        {
            animator.SetBool("isAttacking", true);
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        agent.SetDestination(animator.transform.position);
    }
}
