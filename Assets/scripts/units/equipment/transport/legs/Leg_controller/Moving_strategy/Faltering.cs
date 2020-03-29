using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using UnityEngine;
using rvinowise;
using rvinowise.units.parts.limbs.legs;


namespace rvinowise.units.parts.limbs.legs.strategy {


/* don't try to move constantly - all legs can be raised up:
 bad speed and control
 */
internal class Faltering: Moving_strategy
{
    internal Faltering(IList<Leg> in_legs) : base(in_legs) { }


    internal override void move_on_the_ground(Leg leg) {
        bool can_hold = leg.hold_onto_ground();
        if (
            (leg.is_twisted_badly())||
            (!can_hold)
        ) 
        {
            leg.debug.draw_lines(Color.red);
            leg.raise_up();
        }
    }

    internal override bool belly_touches_ground() {
        return true;
    }
}

}
