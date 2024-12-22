


using rvinowise.contracts;
using rvinowise.unity.extensions;
using rvinowise.unity.geometry2d;
using Unity.Mathematics;
using UnityEngine;


namespace rvinowise.unity.actions {

public class Expose_leg_from_body: Action_sequential_parent {


    private ALeg leg;
    private Transform body;
    //private Quaternion relative_rotation = Quaternion.identity;

    //private float old_provided_impulse = 0;
    
    public static Expose_leg_from_body create(
        ALeg leg,
        Transform body
    ) {
        var action = (Expose_leg_from_body)object_pool.get(typeof(Expose_leg_from_body));
        action.body = body;
        action.leg = leg;
        
        return action;
    }
    public Expose_leg_from_body() {
        
    }

    protected override void on_start_execution() {
        Contract.Assume(leg.is_up(), $"leg {leg} should stay UP while hidden inside body");

        var hideable_leg = leg.GetComponent<Hideable_leg>();
        var relative_rotation_before_hiding =
            (hideable_leg.hiding_direction + new Degree(180f)).to_quaternion();
        var pulling_speed = hideable_leg.pulling_speed;

        var hiding_depth =
            hideable_leg.hiding_depth;

        var rotation_to_optimal_position =
            (leg.optimal_position - (Vector2)leg.transform.position).to_quaternion(); 
        
        add_children(
            Leg_pull_out_segment2.create(
                leg,
                body,
                pulling_speed,
                hiding_depth
            ),
            Leg_pull_out_segment1.create(
                leg,
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