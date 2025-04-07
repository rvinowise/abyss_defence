using System;
using System.Collections.Generic;
using rvinowise.unity.geometry2d;
using rvinowise.unity;
using UnityEngine;

namespace rvinowise.unity.actions {

public class Turn_connected_cogs: Action_parallel_parent {


    private List<Combining_cogs_system.Cog_and_rotation> cogs_with_angles;
    private Degree target_direction;
    
    public static Turn_connected_cogs create(
        List<Combining_cogs_system.Cog_and_rotation> cogs_with_angles

    ) {
        var action = object_pool.get<Turn_connected_cogs>();

        action.cogs_with_angles = cogs_with_angles;

        return action;
    }


    protected override void on_start_execution() {
        foreach (var cog_with_angle in cogs_with_angles) {
            var absolute_angle = cog_with_angle.cog.rotation * new Degree(cog_with_angle.rotation).to_quaternion();
            
            add_child(
                Turning_element_reach_direction.create(
                    cog_with_angle.cog,
                    absolute_angle
                )
            );
        }
        
        base.on_start_execution();
    }
    
    

}

}