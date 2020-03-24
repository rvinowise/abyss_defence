using System;
using System.Collections;
using System.Collections.Generic;
using geometry2d;
using UnityEngine;
using rvinowise;
using rvinowise.units.parts.limbs.arms.actions;
using rvinowise.units.parts.strategy;
using Action = rvinowise.units.parts.limbs.arms.actions.Action;


namespace rvinowise.units.parts.limbs.arms.strategy {

public class Put_hand_before_bag: Action {

    private Baggage bag;

    public static Put_hand_before_bag create(
        Action_tree in_action_tree, Baggage in_bag
    ) {
        var action = (Put_hand_before_bag)pool.get(typeof(Put_hand_before_bag), in_action_tree);
        action.bag = in_bag;
        //action.init(in_bag);
        return action;
    }

    

    public Put_hand_before_bag() {
        
    }

    public override void start() {
        if (arm.held_tool == null) {
            arm.hand.gesture = Hand_gesture.Open_sideview;
        }
    }
    public override void update() {
        Orientation desired_orientation = get_desired_orientation();
        if (complete(desired_orientation)) {
            start_next();
        } else {
            set_desired_directions(desired_orientation);
        }
    }
    
    private void set_desired_directions(Orientation needed_tool_orientation) {
        arm.set_desired_directions_by_position(needed_tool_orientation.position);
        arm.hand.desired_direction = needed_tool_orientation.rotation;
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