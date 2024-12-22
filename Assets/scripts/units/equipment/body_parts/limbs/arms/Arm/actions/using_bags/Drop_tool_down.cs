using rvinowise.unity;

namespace rvinowise.unity.actions {

public class Drop_tool_down: Action_of_arm {

    private Hand hand;
    

    
    public static Drop_tool_down create(
        Arm in_arm
    ) {
        var action = (Drop_tool_down)object_pool.get(typeof(Drop_tool_down));
        action.add_actor(in_arm);
        
        action.hand = in_arm.hand;
        return action;
    }
    public Drop_tool_down() {
        
    }

    protected override void on_start_execution() {
        base.on_start_execution();
    }

    public override void update() {
        base.update();
        if (hand.held_tool != null) {
            Tool tool = hand.detach_tool(); 
        }
        mark_as_completed();
    }


   

}
}