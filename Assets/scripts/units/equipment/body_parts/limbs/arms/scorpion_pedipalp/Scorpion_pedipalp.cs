using System;
using System.Collections.Generic;
using UnityEngine;

using rvinowise.contracts;
using rvinowise.unity.actions;
using rvinowise.unity.extensions;
using rvinowise.unity.geometry2d;
using UnityEngine.Serialization;
using Action = rvinowise.unity.actions.Action;

namespace rvinowise.unity {

public class Scorpion_pedipalp:
    Limb2,
    IAttacker,
    IDefender 
{
    
    public Segment femur => segment1;
    public Scorpion_chila chila;

    public float get_length() {
        return femur.length + chila.length;
    }
    
    public bool is_weapon_ready_for_target(Transform target) {
        var distance_to_target = (target.position - transform.position).magnitude;
        return get_length() >= distance_to_target;
    }

    public void attack(Transform target, System.Action on_completed = null) {
        
    }

    public void start_defence(Transform target, System.Action on_completed) {
    }

    public void finish_defence(System.Action on_completed) {
    }


    public override void on_lacking_action() {
        Scorpion_arm_idle.create(this).start_as_root(actor.action_runner);
    }


    
}

}