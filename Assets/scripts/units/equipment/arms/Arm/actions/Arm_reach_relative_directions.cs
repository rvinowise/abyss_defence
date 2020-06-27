using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rvinowise;
using geometry2d;
using rvinowise.units.parts.actions;
using rvinowise.units.parts.limbs.arms;
using Action = rvinowise.units.parts.actions.Action;

namespace rvinowise.units.parts.limbs.arms.actions {

public class Arm_reach_relative_directions: Action {
    private Arm arm;
    
    public static parts.actions.Action create(
        Action_sequential_parent in_sequential_parent, 
        Arm in_arm, 
        Quaternion upper_arm_rotation,
        Quaternion forearm_rotation,
        Quaternion hand_rotation
    ) {
        var action = (Arm_reach_relative_directions)pool.get(typeof(Arm_reach_relative_directions), in_sequential_parent);
        action.arm = in_arm;

        in_arm.upper_arm.target_direction = new Relative_direction(
            upper_arm_rotation, in_arm.parent.transform
        );
        in_arm.forearm.target_direction = new Relative_direction(
            forearm_rotation, in_arm.upper_arm.transform
        );
        in_arm.hand.target_direction = new Relative_direction(
            hand_rotation, in_arm.forearm.transform
        );
        
        return action;
    }
    public override void update() {
        if (complete()) {
            reached_goal();
        } else {
            arm.rotate_to_desired_directions();
        }
    }

    protected bool complete() {
        //return arm.at_desired_rotation();
        return false;
    }

}
}