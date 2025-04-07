using System;
using System.Collections.Generic;
using rvinowise.unity.geometry2d;
using rvinowise.unity;
using Random = UnityEngine.Random;


namespace rvinowise.unity.actions {

public class Combining_circle_invoke_creature: Action_sequential_parent {

    private Combining_circle combining_circle;
    private float direction_to_exit;
    private Intelligence.Evend_handler on_destroyed_listener;
    
    // public static Combining_circle_invoke_creature create(
    //     Combining_circle combining_circle
    //
    // ) {
    //     var action = object_pool.get<Combining_circle_invoke_creature>();
    //
    //     action.combining_circle = combining_circle;
    //     action.exit_direction = new Degree(Random.value * 360);
    //     
    //     return action;
    // }
    
    public static Combining_circle_invoke_creature create(
        Combining_circle combining_circle,
        float in_exit_direction,
        Intelligence.Evend_handler on_destroyed_listener
    ) {
        var action = object_pool.get<Combining_circle_invoke_creature>();

        action.combining_circle = combining_circle;
        action.direction_to_exit = in_exit_direction;
        action.on_destroyed_listener = on_destroyed_listener;
        return action;
    }


    protected override void on_start_execution() {
        var body_slot = combining_circle.middle_ring.retrieve_random_filled_slot();
        var head_slot = combining_circle.inner_ring.retrieve_random_filled_slot();
        var legs_slot = combining_circle.outer_ring.retrieve_random_filled_slot();

        if (body_slot == null || head_slot == null || legs_slot == null) {
            add_children(Empty_action.create());
        }
        else {
            var creature_intelligence = body_slot.GetComponentInChildren<Intelligence>();
            add_children(
                Combining_circle_arrange_rings_to_direction.create(
                    new List<Tuple<Combining_circle_ring, Combining_circle_slot>> {
                        new Tuple<Combining_circle_ring, Combining_circle_slot>(combining_circle.inner_ring, head_slot),
                        new Tuple<Combining_circle_ring, Combining_circle_slot>(combining_circle.middle_ring,
                            body_slot),
                        new Tuple<Combining_circle_ring, Combining_circle_slot>(combining_circle.outer_ring, legs_slot)
                    },
                    combining_circle.wall,
                    direction_to_exit
                ),
                Attach_unit_parts.create(
                    combining_circle.team,
                    head_slot,
                    body_slot,
                    legs_slot,
                    on_destroyed_listener
                ),
                Open_gate.create(
                    combining_circle.wall.gate
                ),
                Eject_creature.create(
                    creature_intelligence,
                    direction_to_exit-180,
                    combining_circle.ejection_force
                ),
                Close_gate.create(
                    combining_circle.wall.gate
                )
            );
        }
        
        base.on_start_execution();
    }
    
    

}

}