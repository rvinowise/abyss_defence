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

namespace units.human.actions {

public class Start_recorded_animation: Action {

    private Animator animator;
    private int animation_id;
/*    private string animation_name;

    public static Start_recorded_animation create(
        Animator in_animator,
        string in_animation_name
    ) {
        Start_recorded_animation action = (Start_recorded_animation) pool.get(typeof(Start_recorded_animation));
        action.animator = in_animator;
        action.animation_name = in_animation_name;

        return action;
    }*/

    public static Start_recorded_animation create(
        Animator in_animator,
        int in_animation_id
    ) {
        Start_recorded_animation action = (Start_recorded_animation) pool.get(typeof(Start_recorded_animation));
        action.animator = in_animator;
        action.animation_id = in_animation_id;

        return action;
    }

    public override void init_state() {
        animator.enabled = true;
        
        animator.SetTrigger(animation_id);
        //animator.Play("Base Layer.reloading_pistol", 0, 0f);
        
        Reloading_SMB end_notifyer = animator.GetBehaviours<Reloading_SMB>()[0];
        end_notifyer.on_state_exit = this.mark_as_reached_goal;
    }

    public override void restore_state() {
        animator.ResetTrigger(animation_id);
        animator.enabled = false;
    }
    
}
}