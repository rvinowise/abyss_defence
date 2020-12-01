using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using UnityEngine;
using rvinowise.unity.extensions;

using rvinowise;
using rvinowise.unity.units.parts.limbs.creeping_legs;
using rvinowise.unity.units.parts.transport;
using Contract = rvinowise.contracts.Contract;

namespace rvinowise.unity.units.parts.limbs.creeping_legs.strategy {
public abstract class Moving_strategy {

    protected readonly IList<Leg> legs;
    protected readonly Creeping_leg_group creeping_legs_group;
    
    

    protected Moving_strategy(IList<Leg> in_legs, Creeping_leg_group in_creeping_legs_group) {
        legs = in_legs;
        creeping_legs_group = in_creeping_legs_group;
    }

    protected void raise_up(Leg leg) {
        Contract.Requires(!leg.is_up);
        
        leg.raise_up();
        creeping_legs_group.possible_impulse -= leg.provided_impulse;
    }

    //internal abstract void move_legs();
    internal abstract void move_on_the_ground(Leg leg);
    internal abstract bool belly_touches_ground();
}

}
