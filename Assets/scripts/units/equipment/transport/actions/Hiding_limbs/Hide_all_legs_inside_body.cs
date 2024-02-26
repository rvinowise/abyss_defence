


using System.Collections.Generic;
using UnityEngine;


namespace rvinowise.unity.actions {

public class Hide_all_legs_inside_body: Action_parallel_parent {


    private Creeping_leg_group leg_group;
    private Transform body;
    
    public static Hide_all_legs_inside_body create(
        Creeping_leg_group leg_group,
        Transform body
    ) {
        var action = (Hide_all_legs_inside_body)pool.get(typeof(Hide_all_legs_inside_body));
        action.body = body;
        action.leg_group = leg_group;
        
        return action;
    }
    public Hide_all_legs_inside_body() {
        
    }

    protected override void on_start_execution() {
        foreach (var leg in leg_group.legs) {
            add_child(
                Hide_leg_inside_body.create(
                    leg,
                    body
                )
            );
            if (!leg.is_up()) {
                leg_group.moving_strategy.raise_up(leg);
            }
        }
        
        base.on_start_execution();
    }


}
}