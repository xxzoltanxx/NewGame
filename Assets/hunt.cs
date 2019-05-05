using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hunt : StateMachineBehaviour
{
    private Patrollable patrollableComponent;
    private Entity entity;
    private float scanWaitTimer = 0;
    private float scanWaitMax = 3.00f;
    public bool waiting = false;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        patrollableComponent = animator.gameObject.GetComponent<Patrollable>();
        entity = animator.gameObject.GetComponent<Entity>();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Patrollable.PatrolStatus status = patrollableComponent.TickMovement(Time.deltaTime);
        if (patrollableComponent.enterTrigger || (patrollableComponent.GetComponent<Entity>().boundScanner && patrollableComponent.GetComponent<Entity>().boundScanner.enemyEntityScanned && !patrollableComponent.GetComponent<Entity>().boundScanner.enemyEntityScanned.hiddenInPlainSight))
        {
            animator.SetTrigger("attack");
        }
        if (waiting)
        {
            if (patrollableComponent.scanWaitTimer > patrollableComponent.scanDuration * 2.0f)
            { 
                if (!patrollableComponent.lastStateAttack)
                {
                    patrollableComponent.isHunting = false;
                    patrollableComponent.resetToDestinationVillage();
                    animator.SetTrigger("patrol");
                }
                else
                {
                    patrollableComponent.lastStateAttack = false;
                    scanWaitTimer = 0;
                    waiting = false;
                }
            }
        }
        else if (status == Patrollable.PatrolStatus.Finished && !waiting)
        {
            patrollableComponent.triggerScan();
            waiting = true;
            patrollableComponent.scanWaitTimer = 0;
        }
    }
    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

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
