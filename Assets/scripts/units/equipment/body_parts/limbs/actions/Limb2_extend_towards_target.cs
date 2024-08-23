using rvinowise.unity.geometry2d;
using rvinowise.unity;
using rvinowise.unity.extensions;
using UnityEngine;


namespace rvinowise.unity.actions {

public class Limb2_extend_towards_target: Action_leaf {
    
    private Limb2 limb;

    private Transform target;
    
    public static Action create(
        Limb2 in_limb, 
        Transform in_target
    ) {
        
        var action = (Limb2_extend_towards_target)object_pool.get(typeof(Limb2_extend_towards_target));
        
        action.add_actor(in_limb);
        action.limb = in_limb;
        action.target = in_target;
        
        return action;
    }

    protected override void on_start_execution() {
        base.on_start_execution();
        
    }

    public override void update() {

        var rotation_to_target =
            (target.position - limb.transform.position).to_quaternion();
        
        limb.segment1.set_target_rotation(
            rotation_to_target
        ); 
            
        limb.segment2.set_target_rotation(
            rotation_to_target
        );
        
        if (complete()) {
            mark_as_completed();
        } else {
            limb.rotate_to_desired_directions();
        }
    }
  

    protected bool complete() {
        var has_reached_direction = limb.at_desired_rotation();
        var has_bumped_into_border = limb.is_twisted_badly();
        return has_reached_direction || has_bumped_into_border;
    }

}
}