using rvinowise.contracts;
using rvinowise.unity.units.parts.tools;


namespace rvinowise.unity.units.parts.limbs.arms.actions {

public class Grab_ammo: Action_of_arm {

    private Hand hand;
    private Baggage bag;
    private Tool reloaded_tool;
    

    
    public static Grab_ammo create(
        Arm in_arm,
        Baggage in_bag, Tool in_tool
    ) {
        var action = (Grab_ammo)pool.get(typeof(Grab_ammo));
        action.add_actor(in_arm);
        
        action.hand = in_arm.hand;
        action.bag = in_bag;
        action.reloaded_tool = in_tool;
        return action;
    }
   

    
    public override void update() {
        base.update();
        if (hand.held_part != null) {
            stash_old_tool();
        }
        take_ammo_object();
        
        mark_as_completed();
    }

    private void stash_old_tool() {
        Contract.Requires(hand.held_part != null);
        bag.add_tool(hand.held_part.tool);
    }

    private void take_ammo_object() {
        Ammunition ammo = bag.get_ammo_object_for_tool(reloaded_tool);
        hand.switch_held_tools(ammo.main_holding);
    }

   

}
}