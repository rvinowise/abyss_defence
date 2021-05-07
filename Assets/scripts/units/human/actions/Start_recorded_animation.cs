using System;
using System.Collections;
using System.Collections.Generic;
using MoreLinq;
using UnityEngine;
using rvinowise.unity.extensions;

using rvinowise;
using rvinowise.unity.geometry2d;
using rvinowise.unity.units.parts.actions;
using rvinowise.unity.units.parts.limbs.arms;
using Action = rvinowise.unity.units.parts.actions.Action;
using rvinowise.animation;
using rvinowise.unity.units.humanoid;
using rvinowise.contracts;

namespace units {

public class Start_recorded_animation: Action {

    private Animator animator;
    private int animation_name_hash;
    private bool flipped;
    private GameObject object_actor; 
    private IFlippable_actor flippable_actor;

    public static Start_recorded_animation create(
        Animator in_animator,
        int in_animation_name_hash,
        bool in_flipped = false
    ) {
        Contract.Requires(
            in_animator.HasState(0, in_animation_name_hash),
            "animation clip with this ID should exists in the animator"
        );
        Start_recorded_animation action = (Start_recorded_animation) pool.get(typeof(Start_recorded_animation));
        action.animator = in_animator;
        action.animation_name_hash = in_animation_name_hash;
        action.actor = in_animator.GetComponent<IPerform_actions>();
        action.flipped = in_flipped;
        action.object_actor = in_animator.gameObject;
        action.flippable_actor = in_animator.GetComponent<IFlippable_actor>();
        return action;
    }

    public override void init_state() {
        flippable_actor.flip_for_animation(flipped);

        animator.enabled = true;

        animator.Play(animation_name_hash, 0, 0f);
        
        Animation_callback_handler end_notifyer = animator.GetBehaviours<Animation_callback_handler>()[0];
        end_notifyer.on_state_exit = this.mark_as_reached_goal;
        end_notifyer.awaited_animation_name_hash = animation_name_hash;

        /* MonoBehaviour animation_performer = animator.GetComponent<Humanoid>();
        animation_performer.StartCoroutine(wait_for_animation_end()); */
        
    }

 

    public override void restore_state() {
        animator.enabled = false;
        if (flipped) {
            flippable_actor.restore_after_flipping();
            
        }
    }
    
}
}