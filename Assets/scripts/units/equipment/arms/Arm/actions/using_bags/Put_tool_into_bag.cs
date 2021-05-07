using System;
using rvinowise.debug;
using rvinowise.unity.units.parts.actions;
using rvinowise.unity.units.parts.tools;
using UnityEngine;
using rvinowise.unity.extensions;



namespace rvinowise.unity.units.parts.limbs.arms.actions {

public class Put_tool_into_bag: Action_sequential_parent {

    private Arm arm;
    private Baggage bag;
    
    public static Put_tool_into_bag create(
        Arm in_arm,
        Baggage in_bag
    ) {
        var action = (Put_tool_into_bag)pool.get(typeof(Put_tool_into_bag));
        action.actor = in_arm;
        
        action.arm = in_arm;
        action.bag = in_bag;
        action.init_child_actions();
        
        return action;
    }
    


    private void init_child_actions() {
        add_children(
            Put_hand_before_bag.create(arm, bag),
            Move_hand_into_bag.create(arm, bag),
            Drop_tool_into_bag.create(arm, bag)
        );
    
        
    }
    
  
}
}