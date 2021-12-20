using System;
using rvinowise.debug;
using rvinowise.unity.units.parts.actions;
using rvinowise.unity.units.parts.tools;
using UnityEngine;
using rvinowise.unity.extensions;



namespace rvinowise.unity.units.parts.limbs.arms.actions {

public class Take_ammo_from_bag: Action_sequential_parent {

    private Arm arm;
    private Baggage bag;
    private Tool reloaded_tool;
    
    public static Take_ammo_from_bag create(
        Arm in_arm,
        Baggage in_bag, 
        Tool in_for_tool
    ) {
        var action = (Take_ammo_from_bag)pool.get(typeof(Take_ammo_from_bag));
        
        action.arm = in_arm;
        action.bag = in_bag;
        action.reloaded_tool = in_for_tool;
        action.init_child_actions();
        
        return action;
    }
    


    private void init_child_actions() {
        add_children(
            Put_hand_before_bag.create(arm, bag),
            Move_hand_into_bag.create(arm, bag),
            Grab_ammo.create(arm, bag, reloaded_tool),
            Put_hand_before_bag.create(arm, bag)
        );
    
        
    }
    public override void restore_state() {
        Log.info($"{GetType()} Action is ended. ");
    }
    
  
}
}