using rvinowise.contracts;
using rvinowise.unity;

namespace rvinowise.unity.actions {

public class Grab_ammo: Action_of_arm {

    private Hand hand;
    private Baggage bag;
    private Gun reloaded_gun;
    

    
    public static Grab_ammo create(
        Arm in_arm,
        Baggage in_bag, Gun in_gun
    ) {
        var action = (Grab_ammo)object_pool.get(typeof(Grab_ammo));
        action.add_actor(in_arm);
        
        action.hand = in_arm.hand;
        action.bag = in_bag;
        action.reloaded_gun = in_gun;
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
        Ammunition ammo = bag.get_ammo_object_for_gun(reloaded_gun);
        hand.switch_held_tools(ammo.tool.main_holding);
    }

   

}
}