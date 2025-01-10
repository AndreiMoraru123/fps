
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class YBotAttackState : StateMachineBehaviour
{
    Transform player;
    private EnemyWeapon weapon;
    private YBotEnemy enemy;
    NavMeshAgent agent;
    public float stopAttackingDistance = 12.5f;
    public float sightDistance = 18f;
    public float fieldOfView = 85f;
    public float eyeHeight = 1.5f;
    public LayerMask detectionLayers;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        enemy = animator.GetComponent<YBotEnemy>();
        agent = animator.GetComponent<NavMeshAgent>();
        weapon = enemy.GetComponentInChildren<EnemyWeapon>();
        // include everything but the weapon layer
        detectionLayers = Physics.DefaultRaycastLayers & ~(1 << LayerMask.NameToLayer("WeaponRender"));
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (CanSeePlayer())
        {
            LookAtPlayer();
            weapon?.ShootAtTarget(enemy.Player.transform);
        }
        else
        {
            // check if the agent should stop attacking
            var distanceFromPlayer = Vector3.Distance(player.position, animator.transform.position);

            if (distanceFromPlayer > stopAttackingDistance)
            {
                animator.SetBool("isAttacking", false);
            }

        }
    }

    private bool CanSeePlayer()
    {
        if (Vector3.Distance(agent.transform.position, player.position) < sightDistance)
        {
            var targetDirection = player.position - agent.transform.position - (Vector3.up * eyeHeight);
            var angleToPlayer = Vector3.Angle(targetDirection, agent.transform.forward);
            if (angleToPlayer >= -fieldOfView && angleToPlayer <= fieldOfView)
            {
                var ray = new Ray(agent.transform.position + (Vector3.up * eyeHeight), targetDirection);
                if (Physics.Raycast(ray, out RaycastHit hitInfo, sightDistance, detectionLayers))
                {
                    Debug.DrawRay(ray.origin, ray.direction * sightDistance,
                                  hitInfo.transform.gameObject == player.gameObject ? Color.green : Color.red);
                    return hitInfo.transform.gameObject == player.gameObject;
                }
            }
        }
        return false;
    }


    private void LookAtPlayer()
    {
        var direction = player.position - agent.transform.position;
        agent.transform.rotation = Quaternion.LookRotation(direction);

        var yRotation = agent.transform.eulerAngles.y;
        agent.transform.rotation = Quaternion.Euler(0, yRotation, 0);
    }
}
