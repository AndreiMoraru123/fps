
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class YBotAttackState : StateMachineBehaviour
{
    Transform player;
    EnemyWeapon weapon;
    YBotEnemy enemy;
    NavMeshAgent agent;
    public float stopAttackingDistance = 12.5f;
    public float rotationSpeed = 2.5f;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        enemy = animator.GetComponent<YBotEnemy>();
        agent = animator.GetComponent<NavMeshAgent>();
        weapon = enemy.GetComponentInChildren<EnemyWeapon>();
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var distanceFromPlayer = Vector3.Distance(player.position, animator.transform.position);
        // check if the agent should stop attacking (too far or can't see player)
        if (distanceFromPlayer > stopAttackingDistance)
        {
            animator.SetBool("isAttacking", false);
            return;
        }

        AimTowardsPlayer();

        if (enemy.CanSeeTarget(player))
        {
            weapon?.ShootAtTarget(player);
        }
    }


    private void AimTowardsPlayer()
    {
        var direction = player.position - agent.transform.position;
        direction.y = 0;

        if (direction != Vector3.zero)
        {
            var targetRotation = Quaternion.LookRotation(direction);
            agent.transform.rotation = Quaternion.Slerp(
                agent.transform.rotation,
                targetRotation,
                Time.deltaTime * rotationSpeed
            );

            if (weapon != null)
            {
                var weaponDirection = player.position - weapon.transform.position;
                weapon.transform.rotation = Quaternion.LookRotation(weaponDirection);
            }
        }
    }
}
