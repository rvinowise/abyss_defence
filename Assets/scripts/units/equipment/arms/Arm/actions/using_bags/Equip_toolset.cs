using System;
using rvinowise.debug;
using rvinowise.unity.units.parts.actions;
using rvinowise.unity.units.parts.tools;
using UnityEngine;
using rvinowise.unity.extensions;
using rvinowise.unity.ui.input;
using rvinowise.unity.units.humanoid;
using rvinowise.unity.units.parts.limbs.arms.humanoid;
using rvinowise.unity.units.parts.transport;
using Action = rvinowise.unity.units.parts.actions.Action;


namespace rvinowise.unity.units.parts.limbs.arms.actions {

public class Equip_toolset: Action {

    private units.humanoid.Humanoid user;
    private Arm_pair arm_pair;
    private Arm left_arm;
    private Arm right_arm;
    private Baggage baggage;
    private ITransporter transporter;

    private Tool_set toolset;
    
    public static Equip_toolset create(
        Humanoid in_user,
        Tool_set in_toolset
    ) {
        var action = (Equip_toolset)pool.get(typeof(Equip_toolset));
        action.actor = in_user;
        action.user = in_user;
        
        Arm_pair arm_pair = in_user.arm_pair;
        action.arm_pair = arm_pair;
        action.left_arm = arm_pair.left_arm;
        action.right_arm = arm_pair.right_arm;
        action.transporter = arm_pair.transporter;

        action.baggage = in_user.baggage;
        action.toolset = in_toolset;
        
        return action;
    }

    public override void init_state() {
        base.init_state();
        start_equipping_tool_set(toolset);
        mark_as_reached_goal();
    }

    public override void restore_state() {
        Log.info($"{GetType()} Action is ended. ");
    }

    


    public void start_equipping_tool_set(Tool_set in_tool_set) {
        if (weapon_should_change_arms(in_tool_set)) {
            reequip_synchroneousy(in_tool_set);
        } else {
            reequip_asynchroneousy(in_tool_set);
        }
    }

    private bool weapon_should_change_arms(Tool_set tool_set) {
        if (
            (left_arm.held_tool == tool_set.right_tool) ||
            (right_arm.held_tool == tool_set.left_tool)
        ) {
            return true;
        }
        return false;
    }

    private void reequip_asynchroneousy(Tool_set tool_set) {
        if (arms_changing_tools_qty(tool_set) == 1) {
            Debug.Log("reequip_asynchroneousy 1 tool");
        } else {
            Debug.Log("reequip_asynchroneousy 2 tools");
        }

        discard_reloading_actions();
        
        if (left_arm.held_tool != tool_set.left_tool) {
            Take_tool_from_bag.create(
                left_arm,
                baggage,
                tool_set.left_tool
            ).add_marker("changing tool asynch left")
            .start_as_root();
        }
        else if (left_arm.current_action == null) {
            left_arm.start_default_action();
        }
        
        if (right_arm.held_tool != tool_set.right_tool) {
            Take_tool_from_bag.create(
                right_arm,
                baggage,
                tool_set.right_tool
            ).add_marker("changing tool asynch right")
            .start_as_root();
        } else if (right_arm.current_action == null) {
            right_arm.start_default_action();
        }
    }

    private void discard_reloading_actions() {
        if (left_arm.current_action.get_root_action().marker.StartsWith("changing tool")) {
            left_arm.current_action.discard_whole_tree();
        }
        if (right_arm.current_action.get_root_action().marker.StartsWith("changing tool")) {
            right_arm.current_action.discard_whole_tree();
        }
    } 
    
    private int arms_changing_tools_qty(Tool_set tool_set) {
        int qty = 0;
        if (left_arm.held_tool != tool_set.left_tool) {
            qty++;
        }
        if(right_arm.held_tool != tool_set.right_tool) {
            qty++;
        }
        return qty;
    }

    private void reequip_synchroneousy(Tool_set tool_set) {
        if (arms_changing_tools_qty(tool_set) == 1) {
             Debug.Log("reequip_synchroneousy 1 tool");
        }
        else {
            Debug.Log("reequip_synchroneousy 2 tools");
        }

        Action_sequential_parent.create(
            user,
            Action_parallel_parent.create(
                null,
                Put_tool_into_bag.create(
                    left_arm,
                    baggage
                ).add_marker("changing tool"),
                Put_tool_into_bag.create(
                    right_arm,
                    baggage
                ).add_marker("changing tool")
            ),
            Action_parallel_parent.create(
                null,
                Pull_tool_out_of_bag.create(
                    left_arm,
                    baggage,
                    tool_set.left_tool
                ).add_marker("second tier L of [root sequence synch]"),
                Pull_tool_out_of_bag.create(
                    right_arm,
                    baggage,
                    tool_set.right_tool
                ).add_marker("second tier R of [root sequence synch]")
            ).add_marker("second tier")
        ).add_marker("changing tool synch")
        .start_as_root();
    }

    
  
}
}