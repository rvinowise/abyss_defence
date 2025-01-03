﻿using System.Collections.Generic;
using System.Linq;
using rvinowise.unity.actions;
using UnityEngine;


namespace rvinowise.unity {

public class Compound_sensory_organ:
    ISensory_organ 
{

    public IList<ISensory_organ> child_sensors;

    public Compound_sensory_organ(IEnumerable<ISensory_organ> in_child_sensors) {
        child_sensors = in_child_sensors.ToList();
    }
    
    public void pay_attention_to_target(Transform target) {
        foreach (var child_sensor in child_sensors) {
            child_sensor.pay_attention_to_target(target);
        }
    }

    public bool is_focused_on_target() {
        foreach (var child_sensor in child_sensors) {
            if (child_sensor.is_focused_on_target()) {
                return true;
            }
        }
        return false;
    }


    public Actor actor { get; set; }

    public void on_lacking_action() {
        Idle.create(actor).start_as_root(actor.action_runner);
    }

}
}