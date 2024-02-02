using rvinowise.unity.units.parts.actions;


namespace units.human.actions {

public class Idle: Action_leaf {


    
    public static Idle create(   
        IActor actor
    ) {
        Idle action = (Idle)pool.get(typeof(Idle));
        action.add_actor(actor);
        
        return action;
    }
    
}
}