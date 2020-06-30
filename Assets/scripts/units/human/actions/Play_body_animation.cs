using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rvinowise;
using geometry2d;
using rvinowise.units.parts.actions;
using rvinowise.units.parts.limbs.arms;
using rvinowise.units.parts.limbs.arms.actions;
using rvinowise.units.parts.transport;


namespace units.human.actions {

public class Play_body_animation : Action_sequential_parent {


    private Arm left_arm;
    private Arm right_arm;
    private Animator animator;

    public static Play_body_animation create(
        Action_parent in_action_parent,
        Arm in_left_arm,
        Arm in_right_arm,
        Animator in_animator
    ) {
        Play_body_animation action = (Play_body_animation) pool.get(typeof(Play_body_animation), in_action_parent);
        action.left_arm = in_left_arm;
        action.right_arm = in_right_arm;
        action.animator = in_animator;

        action.init_child_actions();
        return action;
    }

    private void init_child_actions() {

        Action_parallel_parent preparing_arms = Action_parallel_parent.create(this);
        preparing_arms.add_child();
            
        child_actions.Enqueue(preparing_arms);
        
        child_actions.Enqueue(
            Start_recorded_animation.create(this, animator)
        );
    }
}
}