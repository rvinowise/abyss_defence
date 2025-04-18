using UnityEngine;
using rvinowise.unity.extensions;

using rvinowise.unity.geometry2d;


namespace rvinowise.unity.actions {


public class Arm_reach_transform: Action_leaf {

    public Arm arm;
    public Transform desired_transform;
    
    public static Action create(
        Arm in_arm, 
        Transform in_desired_orientation
    ) {
        var action = (Arm_reach_transform)object_pool.get(typeof(Arm_reach_transform));
        action.arm = in_arm;
        action.desired_transform = in_desired_orientation;
        action.add_actor(in_arm);
        
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
            return 0.3f;
        }
    }
    public float direction_epsilon = 15f;
    protected virtual bool complete(Transform desired_orientation) {
        if (
            ((Vector2)arm.hand.position - (Vector2)desired_orientation.position).magnitude <= touching_distance  &&
            arm.hand.rotation.abs_degrees_to(desired_orientation.rotation) <= direction_epsilon
        ) 
        {
            return true;
        }
        return false;
    }

}
}