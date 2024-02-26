


using rvinowise.unity.extensions;
using Unity.Mathematics;
using UnityEngine;


namespace rvinowise.unity.actions {

public class Leg_pull_out_segment1: Action_leaf {


    public ALeg leg;
    private Transform body;
    private float pulling_speed;
    private float hiding_depth;

    private Quaternion relative_rotation;
    
    public static Leg_pull_out_segment1 create(
        ALeg leg,
        Transform body,
        float pulling_speed,
        float hiding_depth
    ) {
        var action = (Leg_pull_out_segment1)pool.get(typeof(Leg_pull_out_segment1));
        action.add_actor(leg);
        action.leg = leg;
        action.pulling_speed = pulling_speed;
        action.body = body;
        action.hiding_depth = hiding_depth;
        
        return action;
    }
    public Leg_pull_out_segment1() {
        
    }

    protected override void on_start_execution() {
        base.on_start_execution();
        relative_rotation = Quaternion.Inverse(body.rotation) * leg.segment1.rotation;
    }

    public override void update() {
        base.update();
        
        //segment.rotation = body.rotation * relative_rotation;

        var vector_of_pulling_out =
            leg.segment1.transform.localRotation.to_vector();

        
        //leg.segment2.rotate_to_desired_direction();
        
        if (segment1_is_exposed_completely()) {
            fix_segment1_at_exposed_position();
            mark_as_completed();
        }
        else {
            leg.segment1.transform.localPosition += (Vector3)vector_of_pulling_out * (pulling_speed * Time.deltaTime);
            mark_as_not_completed();
        }
    }

    private void fix_segment1_at_exposed_position() {
        leg.segment1.localPosition = new Vector3(0, 0, leg.segment1.localPosition.z);
        
    }
    
    private bool segment1_is_exposed_completely() {
        return
            leg.segment1.position.distance_to(leg.transform.position) <= pulling_speed*Time.deltaTime*2;
    }
 

}
}