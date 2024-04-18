using System;
using System.Collections.Generic;
using rvinowise.unity.actions;
using UnityEngine;
using Action = rvinowise.unity.actions.Action;

namespace rvinowise.unity {
    
public class Empty_defender:
    IActor_defender 
{

    public void start_defence(Transform target, System.Action on_completed) {
    }

    public void finish_defence(System.Action on_completed) {
        on_completed?.Invoke();
    }

    public void init_for_runner(Action_runner action_runner) {
        
    }

    public Action current_action { get; set; }
    public void on_lacking_action() {
    }

}

}