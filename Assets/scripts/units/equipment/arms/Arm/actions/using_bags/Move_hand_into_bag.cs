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

public class Move_hand_into_bag: Action {

    private Baggage bag;
    private float old_rotation_speed;

    
    public static Move_hand_into_bag create(
        Action_tree in_action_tree, Baggage in_bag
    ) {
        var action = (Move_hand_into_bag)pool.get(typeof(Move_hand_into_bag), in_action_tree);
        action.bag = in_bag;
        return action;
    }
    public Move_hand_into_bag() {
        
    }
    
    public Move_hand_into_bag(Action_tree in_action_tree, Baggage in_bag) : base(in_action_tree) {
        bag = in_bag;
        
    }

    public override void start() {
        slow_movements(arm);
    }

    public override void end() {
        restore_movements(arm);
    }

    private void slow_movements(Arm arm) {
        old_rotation_speed = arm.upper_arm.rotation_speed;
        arm.upper_arm.rotation_speed /= 2f;
        Debug.Log("slow_movements: "+arm.upper_arm.rotation_speed);
    }
    private void restore_movements(Arm arm) {
        arm.upper_arm.rotation_speed = old_rotation_speed;
        Debug.Log("restore_movements: "+arm.upper_arm.rotation_speed);
    }


    public override void update() {
        Orientation desired_orientation = get_orientation_touching_baggage();
        if (complete(desired_orientation)) {
            start_next();
        } else {
            set_desired_directions(arm, desired_orientation);
        }
    }
    
    private void set_desired_directions(Arm arm, Orientation needed_tool_orientation) {
        arm.set_desired_directions_by_position(needed_tool_orientation.position);
        //arm.hand.desired_direction = needed_tool_orientation.rotation;
        arm.hand.desired_direction = Directions.degrees_to_quaternion(
                                         -arm.folding_direction * 90f
                                     ) * 
                                     arm.forearm.desired_direction;
    }
    
    private Orientation get_orientation_touching_baggage() {
        return new Orientation(
            bag.position,// + (Vector2)(aggage.rotation * hand.tip),
            bag.rotation * Directions.degrees_to_quaternion(180f),
            null
        );
    }

    protected bool complete(Orientation desired_orientation) {
        if (
            arm.hand.position == desired_orientation.position /*&&
            arm.hand.rotation.abs_degrees_to(desired_orientation.rotation) < bag.entering_span*/
            ) 
        {
            return true;
        }
        return false;
    }
}
}