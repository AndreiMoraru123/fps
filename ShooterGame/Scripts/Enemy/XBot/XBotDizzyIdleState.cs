using UnityEngine;

public class XBotDizzyIdleState : StateMachineBehaviour
{
    float timer;
    public float blindnessDuration = 10f;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        timer = 0;
        animator.SetBool("isPatrolling", false);
        animator.SetBool("isChasing", false);
        animator.SetBool("isAttacking", false);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        timer += Time.deltaTime;
        if (timer > blindnessDuration)
        {
            animator.ResetTrigger("BLINDED");
        }
    }
}