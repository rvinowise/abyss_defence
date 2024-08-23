using rvinowise.unity;


namespace rvinowise.unity.actions {

public class Take_tool_from_bag: Action_sequential_parent {

    private Arm arm;
    private Baggage bag;
    private Tool tool;
    
    public static Take_tool_from_bag create(
        Arm in_arm,
        Baggage in_bag, 
        Tool in_tool
    ) {
        var action = (Take_tool_from_bag)object_pool.get(typeof(Take_tool_from_bag));
        
        action.arm = in_arm;
        action.bag = in_bag;
        action.tool = in_tool;
        
        return action;
    }

    protected override void on_start_execution() {
        base.on_start_execution();
        init_child_actions();
    }


    private void init_child_actions() {
        if (arm.held_tool is null) {
            arm.hand.set_gesture(Hand_gesture.Open_sideview);
            add_children(
                Put_hand_before_bag.create(arm, bag),
                Move_hand_into_bag.create(arm, bag)
            );
        }
        else {
            add_children(
                Put_tool_into_bag.create(arm, bag)
            );
        }
        add_children(
            Pull_tool_out_of_bag.create(arm, bag, tool)
        );
        
    }

}
}