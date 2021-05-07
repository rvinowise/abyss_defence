using System;
using System.Collections;
using System.Collections.Generic;
using rvinowise.unity.geometry2d;
using UnityEngine;
using rvinowise.unity.extensions;

using rvinowise;
using rvinowise.contracts;
using rvinowise.unity.units.parts.actions;
using rvinowise.unity.units.parts.limbs.arms.actions;
using rvinowise.unity.units.parts.tools;


namespace rvinowise.unity.units.parts.limbs.arms.actions {

public class Drop_tool_into_bag: Action_of_arm {

    private Hand hand;
    private Baggage bag;
    

    
    public static Drop_tool_into_bag create(
        Arm in_arm,
        Baggage in_bag
    ) {
        var action = (Drop_tool_into_bag)pool.get(typeof(Drop_tool_into_bag));
        action.actor = in_arm;
        
        action.hand = in_arm.hand;
        action.bag = in_bag;
        return action;
    }
    public Drop_tool_into_bag() {
        
    }


    public override void init_state() {
        base.init_state();
        if (hand.held_tool != null) {
            drop_tool_into_bag();
        }
        mark_as_reached_goal();
    }

    private void drop_tool_into_bag() {
        Tool tool = hand.detach_tool();
        bag.add_tool(tool);
    }

   

}
}