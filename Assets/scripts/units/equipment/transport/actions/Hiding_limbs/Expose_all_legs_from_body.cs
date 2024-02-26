


using System.Collections.Generic;
using UnityEngine;


namespace rvinowise.unity.actions {

public class Expose_all_legs_from_body: Action_parallel_parent {


    private Creeping_leg_group leg_group;
    private Transform body;
    
    public static Expose_all_legs_from_body create(
        Creeping_leg_group leg_group,
        Transform body
    ) {
        var action = (Expose_all_legs_from_body)pool.get(typeof(Expose_all_legs_from_body));
        action.body = body;
        action.leg_group = leg_group;
        
        return action;
    }
    public Expose_all_legs_from_body() {
        
    }

    protected override void on_start_execution() {
        foreach (var leg in leg_group.legs) {
            add_child(
                Expose_leg_from_body.create(
                    leg,
                    body
                )
            );
            
        }
        
        base.on_start_execution();
    }


}
}