using System;
using System.Collections;
using System.Collections.Generic;
using rvinowise.unity.geometry2d;
using UnityEngine;
using rvinowise.unity.extensions;

using rvinowise;
using rvinowise.unity.units.parts.actions;
using rvinowise.unity.units.parts.limbs.arms.actions;



namespace rvinowise.unity.units.parts.limbs.arms.actions {

public class Put_hand_before_bag: Action_of_arm {

    private Baggage bag;

    public static Put_hand_before_bag create(
        Arm in_arm,
        Baggage in_bag
    ) {
        var action = (Put_hand_before_bag)pool.get(typeof(Put_hand_before_bag));
        action.actor = in_arm;
        
        action.arm = in_arm;
        action.bag = in_bag;
        //action.init(in_bag);
        return action;
    }

    

    public Put_hand_before_bag() {
        
    }

    public override void init_state() {
        base.init_state();

        arm.shoulder_mirrored_target_direction = 60f;

        if (arm.held_tool == null) {
            arm.hand.gesture = Hand_gesture.Open_sideview;
        }
    }
    public override void update() {
        base.update();
        Orientation desired_orientation = get_desired_orientation();
        arm.rotate_to_orientation(desired_orientation);
        if (is_reached_goal(desired_orientation)) {
            mark_as_reached_goal();
        } else {
            mark_as_has_not_reached_goal();
        }
    }
    
    private static Vector2 bag_offset = new Vector2(0.3f,0f);
    private Orientation get_desired_orientation() {
        return new Orientation(
            bag.position + (bag.rotation * bag_offset),
            bag.rotation * Directions.degrees_to_quaternion(180f),
            null
        );
    }

    protected bool is_reached_goal(Orientation desired_orientation) {
        if (
            arm.hand.position.close_enough_to(desired_orientation.position) &&
            arm.hand.rotation.abs_degrees_to(desired_orientation.rotation) < bag.entering_span
        ) 
        {
            return true;
        }
        return false;
    }

}
}