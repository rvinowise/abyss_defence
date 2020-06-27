using System;
using rvinowise.units.parts.actions;
using rvinowise.units.parts.tools;
using UnityEngine;


namespace rvinowise.units.parts.limbs.arms.actions {

public class Take_tool_from_bag: Action_sequential_parent {

    private Arm arm;
    private Baggage bag;
    private Tool tool;
    
    public static Take_tool_from_bag create(
        Action_sequential_parent in_action_sequential_sequence_builder,
        Arm in_arm,
        Baggage in_bag, 
        Tool in_tool
    ) {
        var action = (Take_tool_from_bag)pool.get(typeof(Take_tool_from_bag), in_action_sequential_sequence_builder);
        action.arm = in_arm;
        action.bag = in_bag;
        action.tool = in_tool;
        action.init_child_actions();
        
        return action;
    }
    


    private void init_child_actions() {
        current_child_action_setter = actions.Put_hand_before_bag.create(this, arm, bag);
        new_next_child = actions.Move_hand_into_bag.create(this, arm, bag);
        new_next_child = actions.Grab_tool.create(
            this, arm, bag, tool
        );
        new_next_child = actions.Put_hand_before_bag.create(this, arm, bag);
        
        
    }
    
  
}
}