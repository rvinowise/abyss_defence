﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rvinowise;
using rvinowise.rvi.contracts;
using rvinowise.units.parts.limbs.creeping_legs;
using rvinowise.units.parts.transport;


namespace rvinowise.units.parts.limbs.creeping_legs.strategy {

/* don't try to raise the belly above the ground,
   but keep one leg on the ground to move constantly:
   bad speed but good control.
*/
internal class Grovelling: Moving_strategy
{
    internal Grovelling(IList<Leg> in_legs, Creeping_leg_group in_creeping_legs_group):
        base(in_legs, in_creeping_legs_group) { }

   
    internal override void move_on_the_ground(Leg leg) {
        bool can_hold = leg.hold_onto_ground();
        if (
            (leg.is_twisted_badly())||
            (!can_hold)
        ) 
        {
            leg.debug.draw_lines(Color.red);
            raise_up(leg);

        } 
        else if (leg.is_twisted_uncomfortably()) {
            if (can_move_without(leg)) {
                raise_up(leg);
                
            }
            else {
                if (leg.debug.name == "left_hind_leg") {
                    Debug.Log("left_hind_leg is_twisted_uncomfortably but !can_move_without");
                }
            }
        }
    }

    /* at least one leg will be on he ground if the parameter leg is raised up  */
    private bool can_move_without(Leg in_leg) {
        Contract.Requires(!in_leg.is_up);
        foreach (Leg leg in legs) {
            if (leg == in_leg) {
                continue; 
            }
            if (!leg.is_up) {
                return true;
            }
        }
        return false;
    }

    internal override bool belly_touches_ground() {
        return true;
    }
}

}