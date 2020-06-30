using System;
using System.Collections;
using System.Collections.Generic;
using geometry2d;
using UnityEngine;
using rvinowise;
using rvinowise.units.parts.actions;
using rvinowise.units.parts.limbs.arms.actions;



namespace rvinowise.units.parts.limbs.arms.actions {

public class Put_hand_before_bag: Action_of_arm {

    private Baggage bag;

    public static Put_hand_before_bag create(
        Action_sequential_parent in_action_sequential_parent,
        Arm in_arm,
        Baggage in_bag
    ) {
        var action = (Put_hand_before_bag)pool.get(typeof(Put_hand_before_bag), in_action_sequential_parent);
        action.arm = in_arm;
        action.bag = in_bag;
        //action.init(in_bag);
        return action;
    }

    

    public Put_hand_before_bag() {
        
    }

    public override void init_state() {
        base.init_state();
        if (arm.held_tool == null) {
            arm.hand.gesture = Hand_gesture.Open_sideview;
        }
    }
    public override void update() {
        Orientation desired_orientation = get_desired_orientation();
        if (complete(desired_orientation)) {
            reached_goal();
        } else {
            arm.rotate_to_orientation(desired_orientation);
        }
    }
    
    private static Vector2 bag_offset = new Vector2(0.3f,0f);
    private Orientation get_desired_orientation() {
        return new Orientation(
            bag.position + (Vector2)(bag.rotation * bag_offset),
            bag.rotation * Directions.degrees_to_quaternion(180f),
            null
        );
    }

    protected bool complete(Orientation desired_orientation) {
        if (
            arm.hand.position == desired_orientation.position &&
            arm.hand.rotation.abs_degrees_to(desired_orientation.rotation) < bag.entering_span
        ) 
        {
            return true;
        }
        return false;
    }

}
}