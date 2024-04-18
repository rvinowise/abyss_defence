using System.Collections.Generic;


namespace rvinowise.unity {


/* don't try to move constantly - all legs can be raised up:
 bad speed and control
 */
internal class Faltering: Moving_strategy
{
    internal Faltering(IReadOnlyList<ILeg> in_legs, Creeping_leg_group in_creeping_legs_group):
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
    }

    internal override bool belly_touches_ground() {
        return true;
    }
}

}
