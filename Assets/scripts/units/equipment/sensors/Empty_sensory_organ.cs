using System;
using System.Collections.Generic;
using rvinowise.unity.actions;
using UnityEngine;
using Action = rvinowise.unity.actions.Action;

namespace rvinowise.unity {
    
//[RequireComponent(typeof(Actor))]
public class Empty_sensory_organ:
    MonoBehaviour,
    ISensory_organ 
{

    public static Empty_sensory_organ create(
        Intelligence host    
    ) {
        var tool_object = new GameObject("Empty_sensory_organ");
        var tool_component = tool_object.AddComponent<Empty_sensory_organ>();
        host.action_runner.add_actor(tool_object.AddComponent<Actor>());
        tool_object.transform.parent = host.transform;
        return tool_component;
    }
    
    public void pay_attention_to_target(Transform target) {
        
    }

    public bool is_focused_on_target() {
        return false;
    }


    public Actor actor { get; set; }

    public void on_lacking_action() {
    }

}

}