


using rvinowise.contracts;
using rvinowise.unity.geometry2d;
using Unity.Mathematics;
using UnityEngine;


namespace rvinowise.unity.actions {

public class Hide_leg_inside_body: Action_sequential_parent {


    private ALeg leg;
    private Transform body;
    private Quaternion relative_rotation = Quaternion.identity;

    private float old_provided_impulse = 0;
    
    public static Hide_leg_inside_body create(
        ALeg leg,
        Transform body
    ) {
        var action = (Hide_leg_inside_body)object_pool.get(typeof(Hide_leg_inside_body));
        action.body = body;
        action.leg = leg;
        
        return action;
    }
    public Hide_leg_inside_body() {
        
    }

    protected override void on_start_execution() {
        Debug.Log($"Hide_leg_inside_body::on_start_execution check that Leg is up: {leg}");
        Contract.Assume(leg.is_up(), $"leg {leg.name} should be raised before starting Hide_leg_inside_body");

        var hideable_leg = leg.GetComponent<Hideable_leg>();
        var relative_rotation_before_hiding =
            (hideable_leg.hiding_direction + new Degree(180f)).to_quaternion();
        var pulling_speed = hideable_leg.pulling_speed;

        var hiding_depth =
            hideable_leg.hiding_depth;
        
        add_children(
            Limb_rotate_segment_to_direction.create(
                leg,
                leg.segment1,
                body,
                relative_rotation_before_hiding
            ),
            Leg_pull_inside.create(
                leg,
                leg.segment1,
                body,
                pulling_speed,
                hiding_depth
            ),
            Limb_rotate_segment_to_direction.create(
                leg,
                leg.segment2,
                body,
                relative_rotation_before_hiding
            ),
            Leg_pull_inside.create(
                leg,
                leg.segment2,
                body,
                pulling_speed,
                hiding_depth
            )
        );

        base.on_start_execution();
    }

    // protected override void restore_state() {
    //     base.restore_state();
    //     if (limb is ALeg leg) {
    //         leg.provided_impulse = old_provided_impulse;
    //     } 
    // }

}
}