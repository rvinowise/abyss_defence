using rvinowise.unity.units.parts.tools;


namespace rvinowise.unity.units.parts.limbs.arms.actions {

public class Drop_tool_into_bag: Action_of_arm {

    private Hand hand;
    private Baggage bag;
    

    
    public static Drop_tool_into_bag create(
        Arm in_arm,
        Baggage in_bag
    ) {
        var action = (Drop_tool_into_bag)pool.get(typeof(Drop_tool_into_bag));
        action.add_actor(in_arm);
        
        action.hand = in_arm.hand;
        action.bag = in_bag;
        return action;
    }
    public Drop_tool_into_bag() {
        
    }

    

    public override void update() {
        base.update();
        if (hand.held_tool != null) {
            drop_tool_into_bag();
        }
        mark_as_completed();
    }

    private void drop_tool_into_bag() {
        Tool tool = hand.detach_tool();
        bag.add_tool(tool);
    }

   

}
}