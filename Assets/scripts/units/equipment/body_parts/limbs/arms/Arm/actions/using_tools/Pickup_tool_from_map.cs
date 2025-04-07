using rvinowise.debug;
using rvinowise.unity;
using UnityEngine;


namespace rvinowise.unity.actions {

public class Pickup_tool_from_map: Action_sequential_parent {
    public Tool tool;
    public Arm_pair arm_pair;
    
    public static Pickup_tool_from_map create(
        Arm_pair in_arm_pair,
        Tool in_tool
    ) {
        var action = object_pool.get<Pickup_tool_from_map>();
        action.arm_pair = in_arm_pair;
        action.tool = in_tool;
        return action;
    }


    protected override void on_start_execution() {
        base.on_start_execution();
        var picking_arm = Arm_pair_picking_tool.get_best_arm_for_picking(arm_pair);
        init_child_actions(picking_arm);
    }

    private void init_child_actions(Arm picking_arm) {
        if (picking_arm.held_tool) {
            add_children(
                Put_tool_into_bag.create(picking_arm, arm_pair.baggage)
            );
        }
        
        add_children(
            Arm_reach_transform.create(picking_arm, tool.transform),
            Grab_tool_from_map.create(picking_arm, tool)
        );
        
    }
   
}
}