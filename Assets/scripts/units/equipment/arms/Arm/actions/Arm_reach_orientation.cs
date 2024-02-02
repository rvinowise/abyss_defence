using rvinowise.unity.geometry2d;
using UnityEngine;
using rvinowise.unity.extensions;


namespace rvinowise.unity.units.parts.limbs.arms.actions {

public class Arm_reach_orientation: arms.actions.Action_of_arm {

    public Orientation desired_orientation;
    
    public static parts.actions.Action create(
        Arm in_arm, 
        Orientation in_desired_orientation
    ) {
        var action = (Arm_reach_orientation)pool.get(typeof(Arm_reach_orientation));
        action.arm = in_arm;
        action.desired_orientation = in_desired_orientation;
        
        return action;
    }
    public override void update() {
        base.update();
        if (complete(desired_orientation)) {
            mark_as_completed();
        } else {
            arm.rotate_to_orientation(desired_orientation);
        }
    }


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