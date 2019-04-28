﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : StateMachineBehaviour
{
    public bool visitingLastPosition = false;
    private float timer = 0;
    public float lastSpot = 0.0f;
    public float spotThreshold = 1.5f;
    private Patrollable patrollableComponent;
    private Entity entity;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        patrollableComponent = animator.gameObject.GetComponent<Patrollable>();
        //patrollableComponent.resetToDestinationVillage();
        entity = animator.gameObject.GetComponent<Entity>();
    }

    //OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        lastSpot += Time.deltaTime;
        if (patrollableComponent.enterTrigger && visitingLastPosition && lastSpot < spotThreshold)
        {
            lastSpot = 0.0f;
            visitingLastPosition = false;
            timer = 0;
            animator.SetTrigger("spot");
        }
        if (patrollableComponent.enterTrigger)
        {
            patrollableComponent.didintCheckLastPosition = true;
            patrollableComponent.reset(patrollableComponent.enterTrigger.gameObject.transform.position);
            patrollableComponent.TickMovement(Time.deltaTime);
            patrollableComponent.SetToNormal();
        }
        else if (patrollableComponent.didintCheckLastPosition)
        {
            patrollableComponent.didintCheckLastPosition = false;
            visitingLastPosition = true;
            timer = 0;
            patrollableComponent.reset(patrollableComponent.lastSeenEnemyPosition);
            patrollableComponent.SetToScout();
        }
        else if (visitingLastPosition)
        {
            Patrollable.PatrolStatus status =  patrollableComponent.TickMovement(Time.deltaTime);
            timer += Time.deltaTime;
            if (status == Patrollable.PatrolStatus.Finished)
            {
                visitingLastPosition = false;
                timer = 0;
            }
        }
        else
        {
            animator.SetTrigger("patrol");
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    /*override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Destroy(this);
    }*/

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
