using rvinowise.unity;


namespace rvinowise.unity.actions {

public class Take_reloaded_tool_from_bag: Action_sequential_parent {

    private Arm arm;
    private Baggage bag;
    private Tool tool;
    protected IReloadable reloadable;
    protected Ammo_compatibility ammo;
    
    public static Take_reloaded_tool_from_bag create(
        Arm in_arm,
        Baggage in_bag, 
        Tool in_tool,
        IReloadable reloadable
    ) {
        var action = (Take_reloaded_tool_from_bag)object_pool.get(typeof(Take_reloaded_tool_from_bag));
        
        action.arm = in_arm;
        action.bag = in_bag;
        action.tool = in_tool;
        action.reloadable = reloadable;
        
        return action;
    }
    
    public static Take_reloaded_tool_from_bag create(
        Arm in_arm,
        Baggage in_bag, 
        Tool in_tool,
        IReloadable reloadable,
        Ammo_compatibility ammo
    ) {
        var action = (Take_reloaded_tool_from_bag)object_pool.get(typeof(Take_reloaded_tool_from_bag));
        
        action.arm = in_arm;
        action.bag = in_bag;
        action.tool = in_tool;
        action.reloadable = reloadable;
        action.ammo = ammo;
        
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
            Tacitly_reload_tool.create(bag, reloadable, ammo),
            Pull_tool_out_of_bag.create(arm, bag, tool)
        );
        
    }

}
}