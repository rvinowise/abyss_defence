using System;
using System.Collections.Generic;
using rvinowise.unity.geometry2d;
using rvinowise.unity;
using Random = UnityEngine.Random;


namespace rvinowise.unity.actions {

public class Combining_circle_invoke_creature: Action_sequential_parent {

    private Combining_circle combining_circle;

    
    public static Combining_circle_invoke_creature create(
        Combining_circle combining_circle

    ) {
        var action = (Combining_circle_invoke_creature) object_pool.get(typeof(Combining_circle_invoke_creature));

        action.combining_circle = combining_circle;
        
        return action;
    }


    protected override void on_start_execution() {
        var direction_to_combination = new Degree(Random.value * 360);

        var body_slot = combining_circle.middle_ring.retrieve_random_filled_slot();
        var head_slot = combining_circle.inner_ring.retrieve_random_filled_slot();
        var legs_slot = combining_circle.outer_ring.retrieve_random_filled_slot();
        
        add_children(
            Combining_circle_arrange_rings_to_direction.create(
                new List<Tuple<Combining_circle_ring,Combining_circle_slot>> {
                    new Tuple<Combining_circle_ring,Combining_circle_slot>(combining_circle.inner_ring,head_slot),
                    new Tuple<Combining_circle_ring,Combining_circle_slot>(combining_circle.middle_ring,body_slot),
                    new Tuple<Combining_circle_ring,Combining_circle_slot>(combining_circle.outer_ring,legs_slot)
                },
                combining_circle.wall,
                direction_to_combination
            ),
            Attach_unit_parts.create(
                combining_circle,
                head_slot,
                body_slot,
                legs_slot
            ),
            Open_gate.create(
                combining_circle.wall.gate
            ),
            // Eject_creature.create(
            //
            // ),
            Close_gate.create(
                combining_circle.wall.gate    
            )
        );
        
        base.on_start_execution();
    }
    
    

}

}