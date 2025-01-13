
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
        var playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth.isDead)
        {
            animator.enabled = false;
        }

        var distanceFromPlayer = Vector3.Distance(player.position, animator.transform.position);
        // check if the agent should stop attacking (too far or can't see player)
        if (distanceFromPlayer > stopAttackingDistance)
        {
            animator.SetBool("isAttacking", false);
            return;
        }

        AimTowardsPlayer(animator);

        if (enemy.CanSeeTarget(player))
        {
            weapon?.ShootAtTarget(player);
            agent.updateRotation = false;
        }
        else
        {
            agent.updateRotation = true;
        }
    }


    private void AimTowardsPlayer(Animator animator)
    {
        var direction = player.position - animator.transform.position;
        direction.y = 0;

        if (direction != Vector3.zero)
        {
            var targetRotation = Quaternion.LookRotation(direction);
            animator.transform.rotation = Quaternion.Slerp(
                animator.transform.rotation,
                targetRotation,
                Time.deltaTime * rotationSpeed
            );

            if (weapon != null)
            {
                var weaponDirection = player.position - weapon.transform.position;
                weapon.transform.rotation = Quaternion.LookRotation(weaponDirection);

                var yWeaponRotation = weapon.transform.eulerAngles.y;
                weapon.transform.rotation = Quaternion.Euler(0, yWeaponRotation, 0);
            }
        }
    }
}
