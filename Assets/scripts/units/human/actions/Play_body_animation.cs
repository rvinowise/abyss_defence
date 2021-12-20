using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rvinowise.unity.extensions;

using rvinowise;
using rvinowise.unity.geometry2d;
using rvinowise.unity.units.parts.actions;
using rvinowise.unity.units.parts.limbs.arms;
using rvinowise.unity.units.parts.limbs.arms.actions;
using rvinowise.unity.units.parts.transport;


namespace units.human.actions {

public class Play_body_animation : Action_sequential_parent {


    private Arm left_arm;
    private Arm right_arm;
    private Animator animator;

    public static Play_body_animation create(
        Arm in_left_arm,
        Arm in_right_arm,
        Animator in_animator
    ) {
        Play_body_animation action = (Play_body_animation) pool.get(typeof(Play_body_animation));
        action.left_arm = in_left_arm;
        action.right_arm = in_right_arm;
        action.animator = in_animator;

        action.init_child_actions();
        return action;
    }

    private void init_child_actions() {

        Action_parallel_parent preparing_arms = Action_parallel_parent.create();
        //preparing_arms.add_child();
            
        add_child(preparing_arms);
        
        add_child(
            Start_recorded_animation.create(animator,0)
        );
    }
}
}