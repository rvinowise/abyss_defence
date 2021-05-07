using System;
using rvinowise.contracts;
using rvinowise.debug;
using rvinowise.unity.units.parts.actions;
using rvinowise.unity.units.parts.tools;
using UnityEngine;
using rvinowise.unity.extensions;
using rvinowise.unity.geometry2d;
using rvinowise.unity.ui.input;
using rvinowise.unity.units.humanoid;
using rvinowise.unity.units.parts.limbs.arms.actions.using_guns.reloading;
using rvinowise.unity.units.parts.limbs.arms.humanoid;
using rvinowise.unity.units.parts.transport;
using rvinowise.unity.units.parts.weapons.guns;
using Action = rvinowise.unity.units.parts.actions.Action;


namespace rvinowise.unity.units.parts.limbs.arms.actions {

public class Reload_all: Action_sequential_parent {

    private units.humanoid.Humanoid user;
    private Arm_pair arm_pair;
    private Arm left_arm;
    private Arm right_arm;
    private Baggage baggage;
    private ITransporter transporter;
    
    private Tool left_tool => left_arm.held_tool;
    private Tool right_tool => right_arm.held_tool;

    
    public static Reload_all create(
        Humanoid in_user
    ) {
        var action = (Reload_all)pool.get(typeof(Reload_all));
        action.actor = in_user;
        action.user = in_user;
        
        Arm_pair arm_pair = in_user.arm_pair;
        action.arm_pair = arm_pair;
        action.left_arm = arm_pair.left_arm;
        action.right_arm = arm_pair.right_arm;
        action.transporter = arm_pair.transporter;

        action.baggage = in_user.baggage;
        
        return action;
    }

    public override void init_state() {
        base.init_state();
        reload_all();
    }

    public override void restore_state() {
        Log.info($"{GetType()} Action is ended. ");
    }

    public override void update() {
        base.update();
    }

    public void reload_all() {
        Side first_side = get_side_with_less_ammo();
        Arm gun_arm = arm_pair.get_arm_on_side(first_side);
        Arm ammo_arm = arm_pair.other_arm(gun_arm);
        

        Action first_reloading_action = get_reloading_action_for(first_side);
        Action second_reloading_action = get_reloading_action_for(first_side.flipped());

        add_child(
            Action_sequential_parent.create(
                null,
                first_reloading_action//,
                //second_reloading_action

            )
        );
    }
    
    private Action get_reloading_action_for(Side in_side) {
        Arm gun_arm = arm_pair.get_arm_on_side(in_side);
        Arm ammo_arm = arm_pair.other_arm(gun_arm);
        if (gun_arm.held_tool is Pistol pistol) {

            Ammunition magazine = user.baggage.get_ammo_object_for_tool(pistol);
            Contract.Requires(magazine != null);
            
            return Reload_pistol.create(
                user.animator,
                gun_arm,
                ammo_arm,
                user.baggage,
                pistol
            );
        } else if (gun_arm.held_tool is Pump_shotgun shotgun) {
            return Reload_shotgun.create(
                user.animator,
                gun_arm,
                ammo_arm,
                user.baggage,
                shotgun,
                user.baggage.get_ammo_object_for_tool(shotgun)
            );
        } else if (gun_arm.held_tool is Break_shotgun break_shotgun) {
            return Reload_break_shotgun.create(
                user.animator,
                gun_arm,
                ammo_arm,
                user.baggage,
                break_shotgun,
                user.baggage.get_ammo_object_for_tool(break_shotgun)
            );
        }
        return null;
    }
    
    private Side get_side_with_less_ammo() {
        if (left_tool == null && right_tool == null) {
            return Side.NONE;
        }
        int left_lacking_ammo = (left_tool?.max_ammo_qty - left_tool?.ammo_qty) ?? 0;
        int right_lacking_ammo = (right_tool?.max_ammo_qty - right_tool?.ammo_qty) ?? 0;

        int left_lacking_value = left_lacking_ammo * left_tool.ammo_value;
        int right_lacking_value = right_lacking_ammo * right_tool.ammo_value;

        if (left_lacking_value > right_lacking_value) {
            return Side.LEFT;
        }
        return Side.RIGHT;
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
            Action_sequential_parent.create(
                left_arm,
                Take_tool_from_bag.create(
                    left_arm,
                    baggage,
                    tool_set.left_tool
                )
            ).add_marker("changing tool asynch left")
            .start_as_root();
        }
        else if (left_arm.current_action == null) {
            left_arm.start_default_action();
        }
        
        if (right_arm.held_tool != tool_set.right_tool) {
            Action_sequential_parent.create(
                right_arm,
                Take_tool_from_bag.create(
                    right_arm,
                    baggage,
                    tool_set.right_tool
                )
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
            )
        ).add_marker("changing tool synch")
            .start_as_root();
    }

    
  
}
}