using System;
using System.Collections;
using System.Collections.Generic;
using geometry2d;
using UnityEngine;
using rvinowise;
using rvinowise.rvi.contracts;
using rvinowise.units.parts.actions;
using rvinowise.units.parts.limbs.arms.actions;
using rvinowise.units.parts.tools;


namespace rvinowise.units.parts.limbs.arms.actions {

public class Grab_tool: Action_of_arm {

    private Baggage bag;
    private Tool tool;

    
    public static Grab_tool create(
        Action_sequential_parent in_action_sequential_parent, 
        Arm in_arm,
        Baggage in_bag, Tool in_tool
    ) {
        var action = (Grab_tool)pool.get(typeof(Grab_tool), in_action_sequential_parent);
        action.arm = in_arm;
        action.bag = in_bag;
        action.tool = in_tool;
        return action;
    }
    public Grab_tool() {
        
    }


    public override void init_state() {
        base.init_state();
        if (arm.held_part != null) {
            stash_old_tool();
        }
        if (tool != null) {
            take_new_tool();
        }
        reached_goal();
    }

    private void stash_old_tool() {
        Contract.Requires(arm.held_part != null);
        bag.add_tool(arm.held_part.tool);
    }

    private void take_new_tool() {
        ///bag.remove_tool(tool);
        tool.activate();
        arm.attach_tool_to_hand_for_holding(tool.main_holding);
    }

   

}
}