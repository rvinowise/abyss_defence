using System;
using System.Collections.Generic;
using rvinowise.unity.geometry2d;
using rvinowise.unity;
using UnityEngine;

namespace rvinowise.unity.actions {

public class Combining_circle_arrange_rings_to_direction: Action_parallel_parent {

    // public Combining_circle_ring inner_ring;
    // public Combining_circle_ring middle_ring;
    // public Combining_circle_ring outer_ring;
    private Combining_circle_wall wall;

    private List<Tuple<Combining_circle_ring,Combining_circle_slot>> rings_with_slots;
    private Degree target_direction;
    
    public static Combining_circle_arrange_rings_to_direction create(
        List<Tuple<Combining_circle_ring,Combining_circle_slot>> rings_with_slots,
        Combining_circle_wall wall,
        Degree target_direction

    ) {
        var action = (Combining_circle_arrange_rings_to_direction) object_pool.get(typeof(Combining_circle_arrange_rings_to_direction));

        action.rings_with_slots = rings_with_slots;
        action.wall = wall;
        action.target_direction = target_direction;

        return action;
    }


    protected override void on_start_execution() {
        foreach (var (ring,slot) in rings_with_slots) {
            var ring_target_direction = ring.get_ring_direction_for_slot_direction(slot, target_direction);
            add_child(
                Turning_element_reach_direction.create(
                    ring.turning_element,
                    ring_target_direction
                )
            );
        }
        add_child(
            Turning_element_reach_direction.create(
                wall.turning_element,
                target_direction
            )    
        );
        
        base.on_start_execution();
    }
    
    

}

}