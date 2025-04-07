using System;
using System.Collections.Generic;
using rvinowise.unity.geometry2d;
using rvinowise.unity;
using Random = UnityEngine.Random;


namespace rvinowise.unity.actions {

public class Combining_cogs_invoke_creature: Action_sequential_parent {

    public Combining_cogs_system cog_system;

    
    public static Combining_cogs_invoke_creature create(
        Combining_cogs_system cog_system

    ) {
        var action = object_pool.get<Combining_cogs_invoke_creature>();

        action.cog_system = cog_system;
        
        return action;
    }


    protected override void on_start_execution() {
        var direction_to_combination = new Degree(Random.value * 360);

        var step_angle = (float)360 / 12;
        
        add_children(
            Turn_connected_cogs.create(
                cog_system.cogs_with_rotation
                // new List<Tuple<Turning_element_actor,float>> {
                //     new Tuple<Turning_element_actor,float>(cog_system.cog1,-step_angle),
                //     new Tuple<Turning_element_actor,float>(cog_system.cog2,step_angle)
                // }
            )
            // Attach_unit_parts.create(
            //     cog_system.team,
            //     head_slot,
            //     body_slot,
            //     legs_slot
            // ),
            
        );
        
        base.on_start_execution();
    }
    
    

}

}