using System.Collections.Generic;
using Contract = rvinowise.contracts.Contract;

namespace rvinowise.unity {

public abstract class Moving_strategy {

    protected readonly IReadOnlyList<ILeg> legs;
    protected readonly Creeping_leg_group creeping_legs_group;
    

    protected Moving_strategy(IReadOnlyList<ILeg> in_legs, Creeping_leg_group in_creeping_legs_group) {
        legs = in_legs;
        creeping_legs_group = in_creeping_legs_group;
    }

    public void raise_up(ILeg leg) {
        Contract.Requires(!leg.is_up());
        
        leg.raise_up();
    }

    internal abstract void move_on_the_ground(ILeg leg);
    internal abstract bool belly_touches_ground();
}

}
