using rvinowise.unity.geometry2d;
using rvinowise.unity;
using UnityEngine;


namespace rvinowise.unity.actions {

public class Limb2_reach_relative_directions: Action_leaf {
    private Limb2 limb;

    Degree segment1_relative_direction;
    Degree segment2_relative_direction;
    private Transform relative_to_what;
    
    public static Action create_assuming_left_limb(
        Limb2 in_limb, 
        Degree in_segment1_direction,
        Degree in_segment2_direction,
        Transform relative_to_what
    ) {
        
        var action = (Limb2_reach_relative_directions)object_pool.get(typeof(Limb2_reach_relative_directions));
        
        action.add_actor(in_limb);
        action.limb = in_limb;
        if (in_limb.side == Side_type.LEFT) {
            action.segment1_relative_direction = in_segment1_direction;
            action.segment2_relative_direction = in_segment2_direction;
        } else {
            action.segment1_relative_direction = -in_segment1_direction;
            action.segment2_relative_direction = -in_segment2_direction;
        }
        action.relative_to_what = relative_to_what;
        
        return action;
    }
    

    public override void update() {

        limb.segment1.set_target_rotation(
            relative_to_what.rotation * segment1_relative_direction.to_quaternion()
        ); 
            
        limb.segment2.set_target_rotation(
            limb.segment1.get_target_rotation() * segment2_relative_direction.to_quaternion()
        ); 
        
        if (complete()) {
            mark_as_completed();
        } else {
            limb.rotate_to_desired_directions();
        }
    }
  

    protected bool complete() {
        return limb.at_desired_rotation();
    }

}
}