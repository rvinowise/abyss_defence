using System;
using System.Collections;
using System.Collections.Generic;
using rvinowise.unity.geometry2d;
using UnityEngine;
using rvinowise.unity.extensions;

using rvinowise;


namespace rvinowise.unity.units.parts.limbs.arms.actions {

public abstract class Arm_reach_somewhere: arms.actions.Action_of_arm {

    public override void update() {
        base.update();
        Orientation desired_orientation = get_desired_orientation();
        if (complete(desired_orientation)) {
            mark_as_reached_goal();
        } else {
            arm.rotate_to_orientation(desired_orientation);
        }
    }

    protected abstract Orientation get_desired_orientation();
    protected virtual void set_desired_directions(Arm arm, Orientation needed_orientation) {
        arm.set_desired_directions_by_position(needed_orientation.position);
        arm.hand.target_rotation = needed_orientation.rotation;
    }

    protected virtual float touching_distance{ 
        get{
            return 0.1f;
        }
    }
    protected virtual bool complete(Orientation desired_orientation) {
        if (
            (arm.hand.position - desired_orientation.position).magnitude <= touching_distance  &&
            arm.hand.rotation.abs_degrees_to(desired_orientation.rotation) <= Mathf.Epsilon
        ) 
        {
            return true;
        }
        return false;
    }

}
}