using rvinowise.contracts;
using rvinowise.unity;


namespace rvinowise.unity.actions {

public class Grab_tool: Action_of_arm {

    protected Hand hand;
    protected Baggage bag;
    protected Tool tool;
    

    
    public static Grab_tool create(
        Arm in_arm,
        Baggage in_bag, Tool in_tool
    ) {
        var action = (Grab_tool)object_pool.get(typeof(Grab_tool));
        action.add_actor(in_arm);
        
        action.hand = in_arm.hand;
        action.bag = in_bag;
        action.tool = in_tool;
        return action;
    }
    public Grab_tool() {
        
    }

    
    public override void update() {
        base.update();
        if (hand.held_part != null) {
            stash_old_tool();
        }
        if (tool != null) {
            take_new_tool();
        }
        mark_as_completed();
    }

    protected virtual void stash_old_tool() {
        Contract.Requires(hand.held_part != null);
        Tool old_tool = hand.detach_tool();
        bag.add_tool(old_tool);
    }

    protected virtual void take_new_tool() {
        tool.activate();
        hand.attach_holding_part(tool.main_holding);
    }

   

}
}