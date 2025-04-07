using System.Linq;
using rvinowise.unity.actions;
using rvinowise.unity.geometry2d;
using UnityEngine;


namespace rvinowise.unity {

public static class Arm_pair_picking_tool {


    public static Tool get_hinted_tool() {
        var tools_on_map = Object.FindObjectsByType<Tool>(FindObjectsSortMode.None);
        tools_on_map = tools_on_map.Where(tool => !tool.is_held_by_hand()).ToArray();
        return Finding_objects.find_closest_component(
            Player_input.instance.mouse_world_position,
            tools_on_map
        );
    }

    public static Arm get_only_empty_arm(Arm left_arm, Arm right_arm) {
        bool is_left = left_arm.held_tool == null;
        bool is_right = right_arm.held_tool == null;
        if (is_left == is_right) {
            return null;
        }
        if (is_left) return left_arm;
        return right_arm;
    }
    
    public static Arm get_best_arm_for_picking(Arm_pair arm_pair) {
        Arm left_arm = arm_pair.left_arm;
        Arm right_arm = arm_pair.right_arm;
        
        Arm best_arm = null;
        
        var only_empty_arm = get_only_empty_arm(left_arm, right_arm);
        if (only_empty_arm) {
            best_arm = only_empty_arm;
        }

        var less_loaded_side = Reload_all.get_side_with_less_ammo(left_arm.held_tool, right_arm.held_tool);
        if (less_loaded_side!= Side_type.NONE) {
            best_arm =  Arm_pair_helpers.get_arm_on_side(arm_pair, less_loaded_side);
        }
        
        if (best_arm != null) {
            return best_arm;
        }

        return arm_pair.right_arm;
    }
    
    public static void pick_hinded_tool(Arm_pair arm_pair) {
        var hinted_tool = get_hinted_tool();
        if (hinted_tool == null) return;

        Pickup_tool_from_map.create(arm_pair, hinted_tool).start_as_root(arm_pair.intelligence.action_runner);
    }
    
}

}