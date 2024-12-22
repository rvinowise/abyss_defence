using System.Linq;

using rvinowise.unity;
using rvinowise.unity.extensions;


namespace rvinowise.unity.actions {

public class Use_planting_tool_supertool: Action_sequential_parent {

    private Humanoid user;
    private Arm left_arm;
    private Arm right_arm;
    private Baggage baggage;

    public Tool tool;
    protected IExpendable_equipment expendable_equipment;
    
    public static Use_planting_tool_supertool create(
        Humanoid in_user,
        Tool in_tool,
        IExpendable_equipment expendable_equipment
    ) {
        var action = (Use_planting_tool_supertool)object_pool.get(typeof(Use_planting_tool_supertool));
        action.user = in_user;
        
        Arm_pair arm_pair = action.user.arm_pair;
        action.left_arm = arm_pair.left_arm;
        action.right_arm = arm_pair.right_arm;

        action.baggage = action.user.baggage;
        action.tool = in_tool;
        action.expendable_equipment = expendable_equipment;
        
        return action;
    }

    protected override void on_start_execution() {
        base.on_start_execution();
        Changing_tools.stop_changing_tools(left_arm,right_arm);
        start_using_supertool();
    }

    
    public void start_using_supertool() {
        var closest_arm = Find_best_arm_for_using_tool.get_best_arm_for_supertool(baggage, user.arm_pair);
        use_supertool_with_hand(closest_arm, tool);
    }

    

    private void use_supertool_with_hand(Arm arm, Tool tool) {
        var previous_tool = arm.held_tool;
        add_children(
            Take_copy_of_tool_from_bag.create(
                arm,
                baggage,
                tool,
                expendable_equipment
            ).add_marker("use supertool"),
            Place_tool_down.create(
                arm,
                tool,
                Player_input.instance.cursor.transform
            ),
            Take_tool_from_bag.create(
                arm,
                baggage,
                previous_tool
            ).add_marker("take previous tool after using a supertool supertool")
        );
        
    }
    

    
  
}
}