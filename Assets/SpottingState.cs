using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpottingState : StateMachineBehaviour
{
    Patrollable patrollableComponent = null;
    private Entity entity;
    float timerToWait = 2.0f;
    float timer = 0;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.gameObject.transform.GetChild(2).gameObject.SetActive(true);
        patrollableComponent = animator.gameObject.GetComponent<Patrollable>();
        entity = animator.gameObject.GetComponent<Entity>();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        timer += Time.deltaTime;
        if (timer > timerToWait)
        {
            timer = 0.0f;
            if (!patrollableComponent.enterTrigger)
            {
                animator.SetTrigger("patrol");
            }
            else if (patrollableComponent.enterTrigger.gameObject.GetComponent<Entity>().soldierAmount <= entity.soldierAmount)
            {
                animator.SetTrigger("attack");
            }
            else
            {
                animator.SetTrigger("flee");
            }
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.gameObject.transform.GetChild(2).gameObject.SetActive(false);
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
