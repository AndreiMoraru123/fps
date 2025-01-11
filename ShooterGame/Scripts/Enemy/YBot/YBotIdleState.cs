
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YBotIdleState : StateMachineBehaviour
{

    float timer;
    Transform player;
    YBotEnemy enemy;
    public float idleTime = 0f;
    public float detectionAreaRadius = 18f;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        timer = 0;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        enemy = animator.GetComponent<YBotEnemy>();
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        timer += Time.deltaTime;
        if (timer > idleTime)
        {
            animator.SetBool("isPatrolling", true);
        }

        var distanceFromPlayer = Vector3.Distance(player.position, animator.transform.position);
        if (distanceFromPlayer < detectionAreaRadius && enemy.CanSeeTarget(player))
        {
            animator.SetBool("isChasing", true);
        }
    }
}
