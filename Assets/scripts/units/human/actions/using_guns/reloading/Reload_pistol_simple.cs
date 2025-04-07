using UnityEngine;
using UnityEngine.Events;


namespace rvinowise.unity.actions {

public class Reload_pistol_simple: Action_sequential_parent {
    

    private Baggage bag;
    //private Gun gun;
    private Reloadable reloadable;
    private Arm gun_arm;
    private Arm_pair arm_pair; //mostly for on_ammo_changed;
    
    public static Reload_pistol_simple create(
        Arm_pair arm_pair,
        Arm in_gun_arm,
        Reloadable in_reloadable,
        Baggage in_bag
    ) {
        var action = object_pool.get<Reload_pistol_simple>();
        
        action.gun_arm = in_gun_arm;
        action.arm_pair = arm_pair;
        action.bag = in_bag;
        action.reloadable = in_reloadable;
        action.init_child_actions();
        
        return action;
    }


    private void init_child_actions() {
        add_children(
            Put_hand_before_bag.create(gun_arm, bag),
            Move_hand_into_bag.create(gun_arm, bag),
            Refill_reloadable_tool.create(arm_pair, gun_arm, reloadable, bag),
            Put_hand_before_bag.create(gun_arm, bag)
        );
        

    }

    protected override void on_start_execution() {
        base.on_start_execution();
        var audio_source = reloadable.GetComponent<AudioSource>();
        audio_source?.PlayOneShot(reloadable.eject_magazine_sound);
    }


}
}