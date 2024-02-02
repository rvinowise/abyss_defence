using System.Linq;
using rvinowise.unity.units.parts.actions;
using rvinowise.unity.units.parts.tools;
using rvinowise.unity.units.control.human;
using rvinowise.unity.units.parts.limbs.arms.humanoid;
using rvinowise.unity.units.parts.transport;


namespace rvinowise.unity.units.parts.limbs.arms.actions {

public class Equip_toolset: Action_parallel_parent {

    private units.humanoid.Humanoid user;
    private Arm_pair arm_pair;
    private Arm left_arm;
    private Arm right_arm;
    private Baggage baggage;
    private ITransporter transporter;

    private Toolset toolset;
    
    public static Equip_toolset create(
        Human_intelligence intelligence,
        Toolset in_toolset
    ) {
        var action = (Equip_toolset)pool.get(typeof(Equip_toolset));
        action.user = intelligence.user;
        
        Arm_pair arm_pair = action.user.arm_pair;
        action.arm_pair = arm_pair;
        action.left_arm = arm_pair.left_arm;
        action.right_arm = arm_pair.right_arm;
        action.transporter = arm_pair.transporter;

        action.baggage = action.user.baggage;
        action.toolset = in_toolset;
        
        return action;
    }

    protected override void on_start_execution() {
        stop_changing_tools();
        start_equipping_tool_set(toolset);
    }
    


    public void start_equipping_tool_set(Toolset in_tool_set) {
        
        if (weapon_should_change_arms(in_tool_set)) {
            reequip_synchroneousy(in_tool_set);
        } else {
            reequip_asynchroneousy(in_tool_set);
        }
    }

    private bool weapon_should_change_arms(Toolset tool_set) {
        if (
            (left_arm.held_tool == tool_set.right_tool) ||
            (right_arm.held_tool == tool_set.left_tool)
        ) {
            return true;
        }
        return false;
    }
    
    private bool weapon_should_be_taken(Toolset tool_set) {
        if (
            (left_arm.held_tool == tool_set.right_tool) ||
            (right_arm.held_tool == tool_set.left_tool)
        ) {
            return true;
        }
        return false;
    }

    private void reequip_asynchroneousy(Toolset tool_set) {
        // if (arms_changing_tools_qty(tool_set) == 1) {
        //     Debug.Log("reequip_asynchroneousy 1 tool");
        // } else {
        //     Debug.Log("reequip_asynchroneousy 2 tools");
        // }

        if (left_arm.held_tool != tool_set.left_tool) {
            add_child(
                Take_tool_from_bag.create(
                    left_arm,
                    baggage,
                    tool_set.left_tool
                ).add_marker("changing tool asynch left")
            );
        }
        else if (left_arm.current_action == null) {
            left_arm.on_lacking_action();
        }
        
        if (right_arm.held_tool != tool_set.right_tool) {
            add_child(
                Take_tool_from_bag.create(
                    right_arm,
                    baggage,
                    tool_set.right_tool
                ).add_marker("changing tool asynch right")
            );
        } 
        else if (right_arm.current_action == null) {
            right_arm.on_lacking_action();
        }

        if (!child_actions.Any()) {
            mark_as_completed();
        }
    }
    

    private void stop_changing_tools() {
        if (left_arm.current_action?.get_root_action().marker.StartsWith("changing tool") ?? false) {
            left_arm.current_action.discard_whole_tree();
        }
        if (right_arm.current_action?.get_root_action().marker.StartsWith("changing tool") ?? false) {
            right_arm.current_action.discard_whole_tree();
        }
    } 
    
    private int arms_changing_tools_qty(Toolset tool_set) {
        int qty = 0;
        if (left_arm.held_tool != tool_set.left_tool) {
            qty++;
        }
        if(right_arm.held_tool != tool_set.right_tool) {
            qty++;
        }
        return qty;
    }

    private void reequip_synchroneousy(Toolset tool_set) {
        // if (arms_changing_tools_qty(tool_set) == 1) {
        //      Debug.Log("reequip_synchroneousy 1 tool");
        // }
        // else {
        //     Debug.Log("reequip_synchroneousy 2 tools");
        // }

        add_child(
            Action_sequential_parent.create(
                Action_parallel_parent.create(
                    Put_tool_into_bag.create(
                        left_arm,
                        baggage
                    ).add_marker("changing tool left_arm"),
                    Put_tool_into_bag.create(
                        right_arm,
                        baggage
                    ).add_marker("changing tool right_arm")
                ),
                Action_parallel_parent.create(
                    Pull_tool_out_of_bag.create(
                        left_arm,
                        baggage,
                        tool_set.left_tool
                    ).add_marker("changing tool"),
                    Pull_tool_out_of_bag.create(
                        right_arm,
                        baggage,
                        tool_set.right_tool
                    ).add_marker("changing tool")
                ).add_marker("second tier")
            ).add_marker("changing tool synch")
        );
    }

    
  
}
}