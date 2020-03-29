using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using rvinowise;
using rvinowise.rvi.contracts;
using rvinowise.units.parts.limbs.legs;

namespace rvinowise.units.parts.limbs.legs.strategy {

/* always keep the belly above the ground:
 best speed and control
 */
public class Stable: Moving_strategy
{
    public Stable(IList<Leg> in_legs) : base(in_legs) {
        
    }

    /* legs that are enough to be stable if they are on the ground */
    public IList<Stable_leg_group> stable_leg_groups = new List<Stable_leg_group>();

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
        else if (leg.is_twisted_uncomfortably()) {
            if (is_standing_stable_without(leg)) {
                leg.raise_up();
            }
        }
    }
    
    /* all the legs from at least one stable group will be on the ground
    if the parameter leg is up */
    private bool is_standing_stable_without(Leg leg) {
        Contract.Requires(!leg.is_up);
        foreach (Stable_leg_group stable_leg_group in stable_leg_groups) {
            if (stable_leg_group.contains(leg)) {
                continue; 
            }
            if (stable_leg_group.all_down()) {
                return true;
            }
        }
        return false;
    }

    internal override bool belly_touches_ground() {
        foreach (Stable_leg_group stable_leg_group in stable_leg_groups) {
            if (stable_leg_group.all_down()) {
                return false;
            }
        }
        return true;
    }
}

}
