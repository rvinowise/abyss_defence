using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rvinowise.unity.extensions;


namespace rvinowise.animation {
public class Animation_callback_handler : StateMachineBehaviour {
    internal Action on_state_exit;
    internal int awaited_animation_name_hash;

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
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        var test = animator.GetCurrentAnimatorStateInfo(0);
        var test0 = animator.GetCurrentAnimatorClipInfo(0);

        var test1 = animator.GetNextAnimatorStateInfo(0);
        var test2 = animator.GetNextAnimatorClipInfo(0);
        
        if (stateInfo.fullPathHash == awaited_animation_name_hash) {
            on_state_exit();
        
            Debug.Log("OnStateExit");
        }
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
}