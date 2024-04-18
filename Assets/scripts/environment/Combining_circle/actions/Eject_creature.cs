using System.Collections.Generic;
using rvinowise.unity.geometry2d;
using rvinowise.unity;
using UnityEngine;

namespace rvinowise.unity.actions {

public class Eject_creature: Action_leaf {


    
    public static Eject_creature create(

    ) {
        var action = (Eject_creature) object_pool.get(typeof(Eject_creature));

        
        return action;
    }


    protected override void on_start_execution() {
        
        base.on_start_execution();
    }
    
    

}

}