using System;
using System.Collections;
using System.Collections.Generic;
using geometry2d;
using UnityEngine;
using rvinowise;
using rvinowise.rvi.contracts;
using rvinowise.units.parts.limbs.arms.actions;
using rvinowise.units.parts.strategy;
using rvinowise.units.parts.tools;
using Action = rvinowise.units.parts.limbs.arms.actions.Action;


namespace rvinowise.units.parts.limbs.arms.strategy {

public class Grab_tool: Action {

    private Baggage bag;
    private Tool tool;

    
    public static Grab_tool create(
        Action_tree in_action_tree, Baggage in_bag, Tool in_tool
    ) {
        var action = (Grab_tool)pool.get(typeof(Grab_tool), in_action_tree);
        action.bag = in_bag;
        action.tool = in_tool;
        return action;
    }
    public Grab_tool() {
        
    }


    public override void start() {
        if (arm.held_part != null) {
            stash_old_tool();
        }
        if (tool != null) {
            take_new_tool();
        }
        start_next();
    }

    private void stash_old_tool() {
        Contract.Requires(arm.held_part != null);
        bag.add_tool(arm.held_part.tool);
    }

    private void take_new_tool() {
        //bag.remove_tool(tool);
        tool.activate();
        arm.attach_tool_to_hand_for_holding(tool.main_holding);
    }

   

}
}