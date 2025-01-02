using System;
using System.Collections.Generic;
using UnityEngine;

using rvinowise.contracts;
using rvinowise.unity.actions;
using rvinowise.unity.extensions;
using Action = rvinowise.unity.actions.Action;


namespace rvinowise.unity {

public class Hiding_limbs_group:
    MonoBehaviour
    ,IDefender
{
    public List<ALeg> legs;
    public Creeping_leg_group creeping_leg_group;


    private System.Action intelligence_on_legs_are_hidden;
    private System.Action intelligence_on_legs_are_exposed;
    
    protected void hide_limbs() {
        Hide_all_legs_inside_body.create(
            creeping_leg_group,
            transform
        ).set_on_completed(on_legs_are_hidden)
        .start_as_root(actor.action_runner);
    }

    protected void expose_legs() {
        Expose_all_legs_from_body.create(
            creeping_leg_group,
            transform
        ).set_on_completed(on_legs_are_exposed)
        .start_as_root(actor.action_runner);
    }

    protected void on_legs_are_hidden(actions.Action action) {
        Idle.create(
            actor
        ).start_as_root(actor.action_runner);
        foreach (var leg in creeping_leg_group.legs) {
            Idle.create(leg.actor).start_as_root(actor.action_runner);
        }
        intelligence_on_legs_are_hidden();
    }
    
    protected void on_legs_are_exposed(actions.Action action) {
        foreach (var leg in creeping_leg_group.legs) {
            Creeping_leg_partakes_in_moving.create(leg);
        }
        intelligence_on_legs_are_exposed();
    }
    
    public void start_defence(Transform target, System.Action on_completed) {
        intelligence_on_legs_are_hidden = on_completed;
        hide_limbs();
    }
    
    public void finish_defence(System.Action on_completed) {
        intelligence_on_legs_are_exposed = on_completed;
        expose_legs();
    }

    public Actor actor { get; set; }

    public void on_lacking_action() {
        Idle.create(actor).start_as_root(actor.action_runner);
    }

}

}