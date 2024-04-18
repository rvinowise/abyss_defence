using rvinowise.unity.geometry2d;
using rvinowise.unity;
using rvinowise.unity.extensions;
using UnityEngine;


namespace rvinowise.unity.actions {

public class Limb2_touch_target: Action_leaf {
    protected Limb2 limb;

    protected Transform target;
    
    public static Limb2_touch_target create(
        Limb2 in_limb, 
        Transform in_target
    ) {
        
        var action = (Limb2_touch_target)object_pool.get(typeof(Limb2_touch_target));
        
        action.add_actor(in_limb);
        action.limb = in_limb;
        action.target = in_target;
        
        return action;
    }

    protected override void on_start_execution() {
        base.on_start_execution();
    }

    public override void update() {

        var directions = limb.determine_directions_reaching_point(target.position);
        if (directions.failed) {
            mark_as_completed();
        }
        else if (limb.at_desired_rotation()) {
            mark_as_completed();
        }
        else {
            limb.segment1.set_target_rotation(directions.segment1);
            limb.segment2.set_target_rotation(directions.segment2);
            limb.rotate_to_desired_directions();
        }
        
        
    }

}
}