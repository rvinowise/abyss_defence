using System;
using System.Collections;
using System.Collections.Generic;
using geometry2d;
using UnityEngine;
using rvinowise;


namespace rvinowise.units.parts.limbs.arms.actions {

public abstract class Action_reach_somewhere: arms.actions.Action {

    public override void update() {
        Orientation desired_orientation = get_desired_orientation();
        if (complete(desired_orientation)) {
            start_next();
        } else {
            arm.rotate_to_orientation(desired_orientation);
        }
    }

    protected abstract Orientation get_desired_orientation();
    protected virtual void set_desired_directions(Arm arm, Orientation needed_orientation) {
        arm.set_desired_directions_by_position(needed_orientation.position);
        arm.hand.desired_direction = needed_orientation.rotation;
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