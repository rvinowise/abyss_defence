using System.Collections.Generic;
using rvinowise.unity.geometry2d;
using rvinowise.unity;
using UnityEngine;

namespace rvinowise.unity.actions {

public class Eject_creature: Action_leaf {

    private Intelligence creature_intelligence;
    private Vector2 ejection_vector;
    public float ejection_force = 1000;

    private Rigidbody2D ejected_body;
    
    public static Eject_creature create(
        Intelligence creature_intelligence,
        float direction,
        float ejection_force
    ) {
        var action = (Eject_creature) object_pool.get(typeof(Eject_creature));

        action.creature_intelligence = creature_intelligence;
        action.ejection_vector = new Degree(direction).to_vector();
        action.ejection_force = ejection_force;
        return action;
    }


    protected override void on_start_execution() {
        if (creature_intelligence != null) {
            ejected_body = creature_intelligence.GetComponent<Rigidbody2D>();
        }
        base.on_start_execution();
    }

    public override void update() {
        base.update();
        if (ejected_body != null) {
            ejected_body.AddForce(ejection_vector * ejection_force, ForceMode2D.Impulse);
        }
        else {
            Debug.Log($"can't eject the creature {creature_intelligence}, because it's destroyed");
        }
        mark_as_completed();
    }

}

}