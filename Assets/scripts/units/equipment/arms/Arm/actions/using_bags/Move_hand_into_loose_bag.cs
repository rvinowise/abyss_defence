using System;
using System.Collections;
using System.Collections.Generic;
using geometry2d;
using UnityEngine;
using rvinowise;
using rvinowise.units.parts.limbs.arms.actions;
using rvinowise.units.parts.strategy;
using static geometry2d.Directions;
using Action = rvinowise.units.parts.limbs.arms.actions.Action;

namespace rvinowise.units.parts.limbs.arms.strategy {

public class Move_hand_into_loose_bag: Action {

    private Baggage bag;
    private float old_rotation_speed;

    public Move_hand_into_loose_bag(Action_tree in_action_tree, Baggage in_bag) : base(in_action_tree) {
        bag = in_bag;
    }
    public Move_hand_into_loose_bag() {
        
    }

    public override void start() {
        set_desired_directions(arm);
    }

    public override void end() {
    }




    public override void update() {
        if (complete()) {
            start_next();
        } else {
            set_desired_directions(arm);
            arm.rotate_to_desired_directions();
        }
    }

    private void set_desired_directions(Arm arm) {
        arm.upper_arm.desired_direction =
            arm.upper_arm.parent.rotation *
            degrees_to_quaternion(arm.upper_arm.possible_span.degrees_in_direction(-arm.folding_direction));
        
        arm.forearm.desired_direction =
            arm.upper_arm.desired_direction *
            degrees_to_quaternion(arm.forearm.possible_span.degrees_in_direction(-arm.folding_direction));
        
        arm.hand.desired_direction =
            arm.forearm.desired_direction *
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