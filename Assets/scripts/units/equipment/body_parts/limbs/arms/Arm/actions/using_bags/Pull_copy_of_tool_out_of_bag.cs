using rvinowise.unity;

namespace rvinowise.unity.actions {

/* requires: Move_hand_into_bag */
public class Pull_copy_of_tool_out_of_bag: Action_sequential_parent {

    private Arm arm;
    private Baggage bag;
    private Tool tool;
    protected IExpendable_equipment expendable_equipment;
    
    public static Pull_copy_of_tool_out_of_bag create(
        Arm in_arm,
        Baggage in_bag, 
        Tool in_tool,
        IExpendable_equipment expendable_equipment
    ) {
        var action = (Pull_copy_of_tool_out_of_bag)object_pool.get(typeof(Pull_copy_of_tool_out_of_bag));
        
        action.arm = in_arm;
        action.bag = in_bag;
        action.tool = in_tool;
        action.expendable_equipment = expendable_equipment;
        
        action.init_child_actions();
        
        return action;
    }
    
    private void init_child_actions() {
        add_children(
            Grab_copy_of_tool_from_bag.create(arm, bag, tool,expendable_equipment),
            Put_hand_before_bag.create(arm, bag)
        );
    
        
    }
  
}
}