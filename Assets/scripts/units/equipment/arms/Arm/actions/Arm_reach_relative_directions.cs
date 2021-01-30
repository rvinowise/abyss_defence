using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rvinowise.unity.extensions;

using rvinowise;
using rvinowise.unity.geometry2d;
using rvinowise.unity.units.parts.actions;
using rvinowise.unity.units.parts.limbs.arms;
using Action = rvinowise.unity.units.parts.actions.Action;

namespace rvinowise.unity.units.parts.limbs.arms.actions {

public class Arm_reach_relative_directions: Action {
    private Arm arm;
    
    public static parts.actions.Action create(
        Arm in_arm, 
        Quaternion upper_arm_rotation,
        Quaternion forearm_rotation,
        Quaternion hand_rotation
    ) {
        var action = (Arm_reach_relative_directions)pool.get(typeof(Arm_reach_relative_directions));
        action.actor = in_arm;
        
        action.arm = in_arm;

        in_arm.upper_arm.set_target_direction_relative_to_parent(
            upper_arm_rotation
        );
        in_arm.forearm.set_target_direction_relative_to_parent(
            forearm_rotation
        );
        in_arm.hand.set_target_direction_relative_to_parent(
            hand_rotation
        );
        
        return action;
    }
    
    public static parts.actions.Action create_assuming_left_arm(
        Arm in_arm, 
        float upper_arm_rotation,
        float forearm_rotation,
        float hand_rotation
    ) {
        var action = (Arm_reach_relative_directions)pool.get(typeof(Arm_reach_relative_directions));
        action.actor = in_arm;
        
        action.arm = in_arm;

        in_arm.shoulder.set_target_direction_relative_to_parent(
            in_arm.shoulder.desired_idle_rotation
        );
        in_arm.upper_arm.set_target_direction_relative_to_parent(
            upper_arm_rotation
        );
        in_arm.forearm.set_target_direction_relative_to_parent(
            forearm_rotation
        );
        in_arm.hand.set_target_direction_relative_to_parent(
            hand_rotation
        );
        
        return action;
    }
    public override void update() {
        if (complete()) {
            mark_as_reached_goal();
        } else {
            arm.rotate_to_desired_directions();
        }
    }

    protected bool complete() {
        return arm.at_desired_rotation();
    }

}
}