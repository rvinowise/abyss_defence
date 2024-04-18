using System;
using rvinowise.unity.actions;
using UnityEngine;
using rvinowise.unity.extensions;

using rvinowise.unity.geometry2d;
using static rvinowise.unity.geometry2d.Directions;
using Action = rvinowise.unity.actions.Action;


namespace rvinowise.unity {

public class Turning_element_actor: 
    Turning_element
    ,IActor 
{
    private Action_runner action_runner;
    public void init_for_runner(Action_runner action_runner) {
        this.action_runner = action_runner;
    }

    public Action current_action { get; set; }
    public void on_lacking_action() {
    }
}
}