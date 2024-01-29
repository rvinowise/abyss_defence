using rvinowise.contracts;
using rvinowise.unity.units.parts.tools;


namespace rvinowise.unity.units.parts.limbs.arms.actions {

public class Grab_tool: Action_of_arm {

    private Hand hand;
    private Baggage bag;
    private Tool tool;
    

    
    public static Grab_tool create(
        Arm in_arm,
        Baggage in_bag, Tool in_tool
    ) {
        var action = (Grab_tool)pool.get(typeof(Grab_tool));
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

    private void stash_old_tool() {
        Contract.Requires(hand.held_part != null);
        Tool tool = hand.detach_tool();
        bag.add_tool(tool);
    }

    private void take_new_tool() {
        tool.activate();
        hand.attach_holding_part(tool.main_holding);
    }

   

}
}