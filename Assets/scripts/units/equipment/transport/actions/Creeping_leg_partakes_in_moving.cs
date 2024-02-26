


namespace rvinowise.unity.actions {

public class Creeping_leg_partakes_in_moving: Action_leaf {

    
    public static Creeping_leg_partakes_in_moving create(
        ILeg actor
    ) {
        var action = (Creeping_leg_partakes_in_moving)pool.get(typeof(Creeping_leg_partakes_in_moving));
        action.add_actor(actor);
        
        return action;
    }
    public Creeping_leg_partakes_in_moving() {
        
    }



    
}
}