using UnityEngine;
using rvinowise.unity.extensions;

using rvinowise.unity.geometry2d;
using rvinowise.unity.units.parts.limbs.arms;


namespace units.equipment.parts.actions.Action {


public class Arm_reach_transform: rvinowise.unity.units.parts.actions.Action_leaf {

    public Arm arm;
    public Transform desired_transform;
    
    public static rvinowise.unity.units.parts.actions.Action create(
        Arm in_arm, 
        Transform in_desired_orientation
    ) {
        var action = (Arm_reach_transform)pool.get(typeof(Arm_reach_transform));
        action.arm = in_arm;
        action.desired_transform = in_desired_orientation;
        
        return action;
    }
    public override void update() {
        base.update();
        if (complete(desired_transform)) {
            mark_as_completed();
        } else {
            arm.rotate_to_orientation(Orientation.from_transform(desired_transform));
        }
    }


    protected virtual float touching_distance{ 
        get{
            return 0.1f;
        }
    }
    protected virtual bool complete(Transform desired_orientation) {
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