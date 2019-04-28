using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flee : StateMachineBehaviour
{
    private Patrollable patrollableComponent;
    private Entity entity;
    bool fleeingAfter = false;
    float counter = 0;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        patrollableComponent = animator.gameObject.GetComponent<Patrollable>();
        patrollableComponent.fleeCount += 1;
        //patrollableComponent.resetToDestinationVillage();
        entity = animator.gameObject.GetComponent<Entity>();
    }

    //OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (patrollableComponent.enterTrigger)
        {
            patrollableComponent.RunAway(patrollableComponent.enterTrigger.gameObject.transform.position);
            patrollableComponent.TickMovement(Time.deltaTime);
        }
        else if (patrollableComponent.didintCheckLastPosition)
        {
            patrollableComponent.didintCheckLastPosition = false;
            fleeingAfter = true;
            counter = 0;
        }
        else if (fleeingAfter)
        {
            patrollableComponent.RunAway(patrollableComponent.lastSeenEnemyPosition);
            patrollableComponent.TickMovement(Time.deltaTime);
            counter += Time.deltaTime;
            if (counter > 4.0f)
                fleeingAfter = false;
        }
        else
        {
            animator.SetTrigger("patrol");
        }
    }
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    //override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

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
