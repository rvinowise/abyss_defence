using System;
using rvinowise.debug;
using rvinowise.unity.units.parts.actions;
using rvinowise.unity.units.parts.tools;
using UnityEngine;
using rvinowise.unity.extensions;



namespace rvinowise.unity.units.parts.limbs.arms.actions {

/* requires: Move_hand_into_bag */
public class Pull_tool_out_of_bag: Action_sequential_parent {

    private Arm arm;
    private Baggage bag;
    private Tool tool;
    
    public static Pull_tool_out_of_bag create(
        Arm in_arm,
        Baggage in_bag, 
        Tool in_tool
    ) {
        var action = (Pull_tool_out_of_bag)pool.get(typeof(Pull_tool_out_of_bag));
        action.actor = in_arm;
        
        action.arm = in_arm;
        action.bag = in_bag;
        action.tool = in_tool;
        action.init_child_actions();
        
        return action;
    }
    


    private void init_child_actions() {
        add_children(
            Grab_tool.create(arm, bag, tool),
            Put_hand_before_bag.create(arm, bag)
        );
    
        
    }
    public override void restore_state() {
        Log.info($"{GetType()} Action is ended. ");
    }
    
  
}
}