using System;
using System.Collections;
using System.Collections.Generic;
using rvinowise.unity.geometry2d;
using UnityEngine;
using rvinowise.unity.extensions;

using rvinowise;
using rvinowise.unity.units.parts.actions;
using rvinowise.unity.units.parts.limbs.arms.actions;

using static rvinowise.unity.geometry2d.Directions;

namespace rvinowise.unity.units.parts.limbs.arms.actions {

public class Move_hand_into_loose_bag: Action_of_arm {

    private Baggage bag;
    private float old_rotation_speed;

    public Move_hand_into_loose_bag() {
        
    }

    public override void init_state() {
        base.init_state();
        set_desired_directions(arm);
    }




    public override void update() {
        base.update();
        if (complete()) {
            mark_as_reached_goal();
        } else {
            set_desired_directions(arm);
            arm.rotate_to_desired_directions();
        }
    }

    private void set_desired_directions(Arm arm) {
        arm.upper_arm.target_quaternion =
            arm.upper_arm.parent.rotation *
            degrees_to_quaternion(arm.upper_arm.possible_span.degrees_in_direction(-arm.folding_direction));
        
        arm.forearm.target_quaternion =
            arm.upper_arm.target_quaternion *
            degrees_to_quaternion(arm.forearm.possible_span.degrees_in_direction(-arm.folding_direction));
        
        arm.hand.target_quaternion =
            arm.forearm.target_quaternion *
            degrees_to_quaternion(arm.hand.possible_span.degrees_in_direction(-arm.folding_direction));
        
        /*arm.hand.desired_direction = Directions.degrees_to_quaternion(
                                         arm.folding_direction * (-90f)
                                     ) * 
                                     arm.forearm.desired_direction;*/
    }
    


    protected bool complete() {
        if (
            arm.at_desired_rotation()
        ) 
        {
            return true;
        }
        return false;
    }

    

}
}