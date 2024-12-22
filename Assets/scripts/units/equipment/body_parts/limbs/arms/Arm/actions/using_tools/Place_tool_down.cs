using rvinowise.debug;
using rvinowise.unity;
using UnityEngine;


namespace rvinowise.unity.actions {

public class Place_tool_down: Action_sequential_parent {
    public Tool tool;
    public Arm arm;
    public Transform target_place;
    
    public static Place_tool_down create(
        Arm in_arm,
        Tool in_tool,
        Transform in_target_place
    ) {
        var action = (Place_tool_down)object_pool.get(typeof(Place_tool_down));
        action.arm = in_arm;
        action.tool = in_tool;
        action.target_place = in_target_place;
        return action;
    }


    protected override void on_start_execution() {
        base.on_start_execution();
        init_child_actions();
    }

    private void init_child_actions() {
        add_children(
            Arm_reach_transform.create(arm, target_place),
            Drop_tool_down.create(arm)
        );
        
    }
   
}
}