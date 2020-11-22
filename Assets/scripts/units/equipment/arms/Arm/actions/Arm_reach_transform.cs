using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rvinowise.unity.extensions;

using rvinowise.unity.geometry2d;
using rvinowise.unity.units.parts.actions;
using rvinowise.unity.units.parts.limbs.arms;


namespace units.equipment.parts.actions.Action {


public class Arm_reach_transform: rvinowise.unity.units.parts.actions.Action {

    public Arm arm;
    public Transform desired_transform;
    
    public static rvinowise.unity.units.parts.actions.Action create(
        Action_sequential_parent in_sequential_parent, 
        Arm in_arm, 
        Transform in_desired_orientation
    ) {
        var action = (Arm_reach_transform)pool.get(typeof(Arm_reach_transform), in_sequential_parent);
        action.arm = in_arm;
        action.desired_transform = in_desired_orientation;
        
        return action;
    }
    public override void update() {
        base.update();
        if (complete(desired_transform)) {
            mark_as_reached_goal();
        } else {
            arm.rotate_to_orientation(Orientation.from_transform(desired_transform));
        }
    }

    protected virtual void set_desired_directions(Arm arm, Orientation needed_orientation) {
        arm.set_desired_directions_by_position(needed_orientation.position);
        arm.hand.target_quaternion = needed_orientation.rotation;
    }

    protected virtual float touching_distance{ 
        get{
            return 0.1f;
        }
    }
    protected virtual bool complete(Transform desired_orientation) {
        if (
            (arm.hand.position - (Vector2)desired_orientation.position).magnitude <= touching_distance  &&
            arm.hand.rotation.abs_degrees_to(desired_orientation.rotation) <= Mathf.Epsilon
        ) 
        {
            return true;
        }
        return false;
    }

}
}