using System;
using UnityEngine;


namespace rvinowise.unity {
public class Animation_callback_handler : StateMachineBehaviour {
    internal Action on_state_exit;
    internal int awaited_animation_name_hash;
    internal IFlippable_actor flippable_actor;
   

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        var test = animator.GetCurrentAnimatorStateInfo(0);
        var test0 = animator.GetCurrentAnimatorClipInfo(0);

        var test1 = animator.GetNextAnimatorStateInfo(0);
        var test2 = animator.GetNextAnimatorClipInfo(0);
        
        if (stateInfo.fullPathHash == awaited_animation_name_hash) {
            animator.enabled = false;
            if (flippable_actor!= null && flippable_actor.is_flipped()) {
                flippable_actor.restore_after_flipping();
            }
            on_state_exit();
        
            Debug.Log("OnStateExit");
        }
    }
    
}
}