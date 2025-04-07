using System;
using System.Collections.Generic;
using rvinowise.unity.actions;
using UnityEngine;
using Action = rvinowise.unity.actions.Action;

namespace rvinowise.unity {
    
//[RequireComponent(typeof(Actor))]
public class Empty_defender:
    MonoBehaviour,
    IDefender 
{

    
    public static Empty_defender create(
        Intelligence host    
    ) {
        var tool_object = new GameObject("Empty_defender");
        var tool_component = tool_object.AddComponent<Empty_defender>();
        host.action_runner.add_actor(tool_object.AddComponent<Actor>());
        tool_object.transform.parent = host.transform;
        return tool_component;
    }
    public void start_defence(Transform target, System.Action on_completed) {
    }

    public void finish_defence(System.Action on_completed) {
        on_completed?.Invoke();
    }

    public void init_for_runner(Action_runner action_runner) {
        
    }

    public Actor actor { get; set; }

    public void on_lacking_action() {
    }

}

}