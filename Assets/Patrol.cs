﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Patrol : StateMachineBehaviour
{
    private Patrollable patrollableComponent;
    private Entity entity;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        patrollableComponent = animator.gameObject.GetComponent<Patrollable>();
        patrollableComponent.resetToDestinationVillage();
        entity = animator.gameObject.GetComponent<Entity>();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (patrollableComponent.enterTrigger && entity.soldierAmount >= patrollableComponent.enterTrigger.gameObject.GetComponent<Entity>().soldierAmount)
        {
            animator.SetTrigger("attack");
        }
        else if (patrollableComponent.enterTrigger && entity.soldierAmount < patrollableComponent.enterTrigger.gameObject.GetComponent<Entity>().soldierAmount)
        {
            animator.SetTrigger("flee");
        }
        else
        {
            Patrollable.PatrolStatus status = patrollableComponent.TickMovement(Time.deltaTime);
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
