using System.Collections.Generic;
using rvinowise.unity.geometry2d;
using rvinowise.unity;
using UnityEngine;

namespace rvinowise.unity.actions {

public class Close_gate: Action_leaf {

    private Gate gate;

    
    public static Close_gate create(
        Gate gate

    ) {
        var action = (Close_gate) object_pool.get(typeof(Close_gate));

        action.gate = gate;
        
        return action;
    }


    protected override void on_start_execution() {
        gate.on_closed_listeners += this.mark_as_completed;
        gate.start_closing();
        
        base.on_start_execution();
    }
    
    

}

}