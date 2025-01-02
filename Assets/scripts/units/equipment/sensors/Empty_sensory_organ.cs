using System;
using System.Collections.Generic;
using rvinowise.unity.actions;
using UnityEngine;
using Action = rvinowise.unity.actions.Action;

namespace rvinowise.unity {
    
public class Empty_sensory_organ:
    ISensory_organ 
{

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