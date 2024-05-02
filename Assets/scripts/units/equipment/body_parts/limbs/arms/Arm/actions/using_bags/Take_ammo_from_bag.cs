using rvinowise.unity;


namespace rvinowise.unity.actions {

public class Take_ammo_from_bag: Action_sequential_parent {

    private Arm arm;
    private Baggage bag;
    private Gun reloaded_gun;
    
    public static Take_ammo_from_bag create(
        Arm in_arm,
        Baggage in_bag, 
        Gun in_for_tool
    ) {
        var action = (Take_ammo_from_bag)object_pool.get(typeof(Take_ammo_from_bag));
        
        action.arm = in_arm;
        action.bag = in_bag;
        action.reloaded_gun = in_for_tool;
        action.init_child_actions();
        
        return action;
    }
    


    private void init_child_actions() {
        add_children(
            Put_hand_before_bag.create(arm, bag),
            Move_hand_into_bag.create(arm, bag),
            Grab_ammo.create(arm, bag, reloaded_gun),
            Put_hand_before_bag.create(arm, bag)
        );
    
        
    }

    
  
}
}