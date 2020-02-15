using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using UnityEngine;
using rvinowise;


namespace units {
namespace limbs {
namespace strategy {

internal abstract class Moving_strategy {

    protected readonly IList<Leg> legs;

    protected Moving_strategy(IList<Leg> in_legs) {
        legs = in_legs;
    }

    //internal abstract void move_legs();
    internal abstract void move_on_the_ground(Leg leg);
    internal abstract bool belly_touches_ground();
}

}
}
}