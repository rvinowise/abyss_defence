using System;
using System.Collections.Generic;
using rvinowise.unity.actions;
using UnityEngine;
using Action = rvinowise.unity.actions.Action;

namespace rvinowise.unity {
    
public class Empty_defender:
    IDefender 
{

    public void start_defence(Transform target, System.Action on_completed) {
    }

    public void finish_defence(System.Action on_completed) {
        on_completed?.Invoke();
    }
    
}

}