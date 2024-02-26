


using rvinowise.unity.extensions;
using Unity.Mathematics;
using UnityEngine;


namespace rvinowise.unity.actions {

public class Leg_pull_inside: Action_leaf {


    public ALeg leg;
    public Segment segment;
    private Transform body;
    private float pulling_speed;
    //private Quaternion hiding_relative_rotation;
    private float hiding_depth;

    private Quaternion relative_rotation;
    
    public static Leg_pull_inside create(
        ALeg leg,
        Segment segment,
        Transform body,
        float pulling_speed,
        //Quaternion hiding_relative_rotation,
        float hiding_depth
    ) {
        var action = (Leg_pull_inside)pool.get(typeof(Leg_pull_inside));
        action.add_actor(leg);
        action.leg = leg;
        action.pulling_speed = pulling_speed;
        action.body = body;
        action.segment = segment;
        //action.hiding_relative_rotation = hiding_relative_rotation;
        action.hiding_depth = hiding_depth;
        
        return action;
    }
    public Leg_pull_inside() {
        
    }

    protected override void on_start_execution() {
        base.on_start_execution();
        relative_rotation = Quaternion.Inverse(body.rotation) * segment.rotation;
    }

    public override void update() {
        base.update();
        
        segment.rotation = body.rotation * relative_rotation;

        if (segment == leg.segment1) {
            leg.segment2.target_rotation = leg.segment1.rotation;
            leg.segment2.rotate_to_desired_direction();
        }
        
        if (segment_is_hidden_completely()) {
            mark_as_completed();
        }
        else {
            mark_as_not_completed();
            
            var vector_of_hiding =
                -segment.transform.localRotation.to_vector();

            segment.transform.localPosition += (Vector3)vector_of_hiding * (pulling_speed * Time.deltaTime);
        }
        
        
    }

    private bool segment_is_hidden_completely() {
        return
            segment.position.distance_to(leg.transform.position) >= hiding_depth;
    }


}
}