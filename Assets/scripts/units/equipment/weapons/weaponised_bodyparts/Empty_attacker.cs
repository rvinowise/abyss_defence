using System;
using System.Collections.Generic;
using rvinowise.unity.actions;
using UnityEngine;
using Action = rvinowise.unity.actions.Action;

namespace rvinowise.unity {
    
public class Empty_attacker:
    IActor_attacker 
{

    //public Transform transform { get; }

    public bool is_weapon_targeting_target(Transform target) {
        return false;
    }

    public float get_reaching_distance() => 0f;
    
    public void attack(Transform target, System.Action on_completed = null) {
        on_completed?.Invoke();
    }

    public void init_for_runner(Action_runner action_runner) {
        
    }

    public Action current_action { get; set; }
    public void on_lacking_action() {
    }

}

}