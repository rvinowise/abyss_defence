using System;
using System.Collections;
using System.Collections.Generic;
using geometry2d;
using UnityEngine;
using rvinowise;
using rvinowise.units.parts.actions;
using rvinowise.units.parts.limbs.arms.actions;
using rvinowise.units.parts.strategy;
using units.equipment.arms.Arm.actions;


namespace rvinowise.units.parts.limbs.arms.actions {

public class Move_hand_into_bag: Action_of_arm {

    private Baggage bag;
    private float old_rotation_speed;

    
    public static Move_hand_into_bag create(
        Action_parent in_action_parent, 
        Arm in_arm,
        Baggage in_bag
    ) {
        var action = (Move_hand_into_bag)pool.get(typeof(Move_hand_into_bag), in_action_parent);
        action.arm = in_arm;
        action.bag = in_bag;
        return action;
    }
    public Move_hand_into_bag() {
        
    }
    

    public override void init_state() {
        base.init_state();
        slow_movements(arm);
    }

    public override void restore_state() {
        restore_movements(arm);
        base.restore_state();
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
            transition_to_next_action();
        } else {
            arm.rotate_to_orientation(desired_orientation);
        }
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