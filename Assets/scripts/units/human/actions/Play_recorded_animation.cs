using UnityEngine;
using rvinowise.contracts;

using rvinowise.unity;

namespace rvinowise.unity.actions {

public class Play_recorded_animation: Action_leaf {

    private Animator animator;
    private int animation_name_hash;
    private bool flipped;
    private GameObject object_actor; 
    private IFlippable_actor flippable_actor;

    public static Play_recorded_animation create(
        Animator in_animator,
        int in_animation_name_hash,
        bool in_flipped = false
    ) {
        Contract.Requires(
            in_animator.HasState(0, in_animation_name_hash),
            "animation clip with this ID should exists in the animator"
        );
        Play_recorded_animation action = (Play_recorded_animation) object_pool.get(typeof(Play_recorded_animation));
        action.animator = in_animator;
        action.animation_name_hash = in_animation_name_hash;
        //action.actor = in_animator.GetComponent<IActor>();
        action.flipped = in_flipped;
        action.object_actor = in_animator.gameObject;
        action.flippable_actor = in_animator.GetComponent<IFlippable_actor>();
        return action;
    }

    protected override void on_start_execution() {
        flippable_actor?.flip_for_animation(flipped);

        animator.enabled = true;

        animator.Play(animation_name_hash, 0, 0f);
        
        Animation_callback_handler end_notifyer = animator.GetBehaviours<Animation_callback_handler>()[0];
        end_notifyer.on_state_exit = mark_as_completed;
        end_notifyer.awaited_animation_name_hash = animation_name_hash;
        end_notifyer.flippable_actor = flippable_actor;

    }

 

    
}
}