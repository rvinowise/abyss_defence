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

public class Move_hand_into_bag: Action_of_arm {

    private Baggage bag;
    private float old_upper_arm_rotation_speed;
    private float old_shoulder_rotation_speed;

    
    public static Move_hand_into_bag create(
        Arm in_arm,
        Baggage in_bag
    ) {
        var action = (Move_hand_into_bag)pool.get(typeof(Move_hand_into_bag));
        action.actor = in_arm;
        
        action.arm = in_arm;
        action.bag = in_bag;
        return action;
    }
    public Move_hand_into_bag() {
        
    }
    

    public override void init_state() {
        base.init_state();
        
        arm.shoulder_mirrored_target_direction = 45f;

        slow_movements(arm);
    }

    public override void restore_state() {
        restore_movements(arm);
        base.restore_state();
    }

    private void slow_movements(Arm arm) {
        old_upper_arm_rotation_speed = arm.upper_arm.rotation_speed;
        arm.upper_arm.rotation_speed /= 2f;
        
        old_shoulder_rotation_speed = arm.shoulder.rotation_speed;
        arm.shoulder.rotation_speed /= 2f;
        
    }
    private void restore_movements(Arm arm) {
        arm.upper_arm.rotation_speed = old_upper_arm_rotation_speed;
        arm.shoulder.rotation_speed = old_shoulder_rotation_speed;
    }


    public override void update() {
        base.update();
        Orientation desired_orientation = get_orientation_touching_baggage();
        arm.rotate_to_orientation(desired_orientation);
        if (complete(desired_orientation)) {
            mark_as_reached_goal();
        } else {
            mark_as_has_not_reached_goal();
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
            arm.hand.position.close_enough_to(desired_orientation.position) /*&&
            arm.hand.rotation.abs_degrees_to(desired_orientation.rotation) < bag.entering_span*/
            ) 
        {
            return true;
        }
        return false;
    }
}
}