using System.Collections.Generic;
using UnityEngine;
using rvinowise.contracts;


namespace rvinowise.unity {

/* don't try to raise the belly above the ground,
   but keep one leg on the ground to move constantly:
   bad speed but good control.
*/
internal class Grovelling: Moving_strategy
{
    internal Grovelling(IReadOnlyList<ILeg> in_legs, Creeping_leg_group in_creeping_legs_group):
        base(in_legs, in_creeping_legs_group) { }

   
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
            if (can_move_without(leg)) {
                raise_up(leg);
            }
        }
    }

    /* at least one leg will be on he ground if the parameter leg is raised up  */
    private bool can_move_without(ILeg in_leg) {
        Contract.Requires(!in_leg.is_up());
        foreach (ILeg leg in legs) {
            if (leg == in_leg) {
                continue; 
            }
            if (!leg.is_up()) {
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
