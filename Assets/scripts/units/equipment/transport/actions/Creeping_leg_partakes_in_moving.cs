using rvinowise.unity.units.parts.actions;
using rvinowise.unity.units.parts.limbs.creeping_legs;


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