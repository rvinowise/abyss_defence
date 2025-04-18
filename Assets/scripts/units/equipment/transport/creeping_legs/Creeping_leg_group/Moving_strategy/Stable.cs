﻿using System.Collections.Generic;
using UnityEngine;
using rvinowise.contracts;


namespace rvinowise.unity {

/* always keep the belly above the ground:
 best speed and control
 */
public class Stable: Moving_strategy
{

    public Stable(IReadOnlyList<ILeg> in_legs, Creeping_leg_group in_creeping_legs_group) :
        base(in_legs, in_creeping_legs_group) 
    {
        init_stable_leg_groups();
    }

    private void init_stable_leg_groups() {
        foreach (var stable_leg_group in creeping_legs_group.stable_leg_groups) {
            foreach (var leg in stable_leg_group.legs) {
                leg.stable_group = stable_leg_group;
            }
        }
    }
    

    internal override void move_on_the_ground(ILeg leg) {
        bool can_hold = leg.hold_onto_ground();
        if (
            (leg.is_twisted_badly())||
            (!can_hold)
        ) 
        {
            raise_up(leg);
        } 
        else if (leg.is_twisted_uncomfortably()) {
            if (is_standing_stable_without(leg)) {
                raise_up(leg);
            }
        }
    }
    
    /* all the legs from at least one stable group will be on the ground
    if the parameter leg is up */
    private bool is_standing_stable_without(ILeg leg) {
        Contract.Requires(!leg.is_up());
        foreach (Stable_leg_group stable_leg_group in creeping_legs_group.stable_leg_groups) {
            if (leg.stable_group == stable_leg_group) {
                continue; 
            }
            if (stable_leg_group.all_down()) {
                return true;
            }
        }
        return false;
    }

    internal override bool belly_touches_ground() {
        foreach (Stable_leg_group stable_leg_group in creeping_legs_group.stable_leg_groups) {
            if (stable_leg_group.all_down()) {
                return false;
            }
        }
        return true;
    }
}

}
