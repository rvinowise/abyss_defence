


using rvinowise.unity.extensions;
using Unity.Mathematics;
using UnityEngine;


namespace rvinowise.unity.actions {

public class Limb_rotate_segment_to_direction: Action_leaf {


    public Limb2 limb;
    public Segment segment;
    private Quaternion relative_rotation;
    private Transform body;
    
    public static Limb_rotate_segment_to_direction create(
        Limb2 limb,
        Segment segment,
        Transform body,
        Quaternion relative_rotation
    ) {
        var action = (Limb_rotate_segment_to_direction)pool.get(typeof(Limb_rotate_segment_to_direction));
        action.add_actor(limb);
        action.limb = limb;
        action.relative_rotation = relative_rotation;
        action.body = body;
        action.segment = segment;
        
        return action;
    }
    public Limb_rotate_segment_to_direction() {
        
    }

    protected override void on_start_execution() {
        base.on_start_execution();
        
    }

    public override void update() {
        base.update();
        limb.segment1.target_rotation = body.rotation * relative_rotation;
        limb.segment2.target_rotation = limb.segment1.rotation;
        limb.rotate_to_desired_directions();
        if (segment_has_reached_direction()) {
            mark_as_completed();
        }
        else {
            mark_as_not_completed();
        }
    }

    private bool segment_has_reached_direction() {
        return 
            //Mathf.Abs(segment.rotation.degrees_to(body.rotation * relative_rotation))  
            Mathf.Abs(segment.rotation.degrees_to(segment.target_rotation))
            <=
            Turning_element.rotation_epsilon;
    }


}
}