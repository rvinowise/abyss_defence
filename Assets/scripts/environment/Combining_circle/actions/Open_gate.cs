using System.Collections.Generic;
using rvinowise.unity.geometry2d;
using rvinowise.unity;
using UnityEngine;

namespace rvinowise.unity.actions {

public class Open_gate: Action_leaf {

    private Gate gate;

    
    public static Open_gate create(
        Gate gate

    ) {
        var action = (Open_gate) object_pool.get(typeof(Open_gate));

        action.gate = gate;
        
        return action;
    }


    protected override void on_start_execution() {
        gate.on_opened_listeners += this.mark_as_completed;
        gate.start_opening();
        
        base.on_start_execution();
    }
    
    

}

}