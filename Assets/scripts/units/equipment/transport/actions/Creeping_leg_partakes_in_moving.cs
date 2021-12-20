using System;
using System.Collections;
using System.Collections.Generic;
using rvinowise.unity.geometry2d;
using UnityEngine;
using rvinowise.unity.extensions;

using rvinowise;
using rvinowise.unity.units.parts.actions;
using rvinowise.unity.units.parts.limbs.arms.actions;
using rvinowise.unity.units.parts.limbs.creeping_legs;
using rvinowise.unity.units.parts.transport;
using Action = rvinowise.unity.units.parts.actions.Action;


namespace rvinowise.unity.units.parts.limbs.arms.actions {

public class Creeping_leg_partakes_in_moving: Action_leaf {

    
    public static Creeping_leg_partakes_in_moving create(
        ILeg actor
    ) {
        var action = (Creeping_leg_partakes_in_moving)pool.get(typeof(Creeping_leg_partakes_in_moving));
        action.actor = actor;
        
        return action;
    }
    public Creeping_leg_partakes_in_moving() {
        
    }



    
}
}