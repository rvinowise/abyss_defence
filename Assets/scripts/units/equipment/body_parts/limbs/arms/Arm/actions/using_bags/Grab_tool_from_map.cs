using rvinowise.contracts;
using rvinowise.unity;


namespace rvinowise.unity.actions {

public class Grab_tool_from_map: Action_of_arm {

    protected Hand hand;
    protected Tool tool;
    

    
    public static Grab_tool_from_map create(
        Arm in_arm,
        Tool in_tool
    ) {
        var action =object_pool.get<Grab_tool_from_map>();
        action.add_actor(in_arm);
        
        action.hand = in_arm.hand;
        action.tool = in_tool;
        return action;
    }
    public Grab_tool_from_map() {
        
    }

    
    public override void update() {
        base.update();
        if (hand.held_part != null) {
            drop_old_tool();
        }
        if (tool != null) {
            take_new_tool();
        }
        mark_as_completed();
    }

    protected virtual void drop_old_tool() {
        hand.detach_tool();
    }

    protected virtual void take_new_tool() {
        hand.attach_holding_part(tool.main_holding);
    }

   

}
}