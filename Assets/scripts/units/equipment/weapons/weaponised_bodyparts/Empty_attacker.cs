using System;
using System.Collections.Generic;
using System.Linq;
using rvinowise.unity.actions;
using UnityEngine;
using Action = rvinowise.unity.actions.Action;

namespace rvinowise.unity {
    
//[RequireComponent(typeof(Actor))]
public class Empty_attacker:
    MonoBehaviour,
    IAttacker 
{

    public static Empty_attacker create(
        Intelligence host    
    ) {
        var tool_object = new GameObject("Empty_attacker");
        var tool_component = tool_object.AddComponent<Empty_attacker>();
        host.action_runner.add_actor(tool_object.AddComponent<Actor>());
        tool_object.transform.parent = host.transform;
        return tool_component;
    }

    public bool is_weapon_ready_for_target(Transform target) {
        return false;
    }

    public IEnumerable<Damage_receiver> get_targets() {
        return Enumerable.Empty<Damage_receiver>();
    }

    public float get_reaching_distance() => 0f;
    
    public void attack(Transform target, System.Action on_completed = null) {
        on_completed?.Invoke();
    }


    public Actor actor { get; set; }

    public void on_lacking_action() {
    }

}

}