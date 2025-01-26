using UnityEngine;
using UnityEngine.Events;


namespace rvinowise.unity.actions {

public class Reload_pistol_simple: Action_sequential_parent {
    

    private Baggage bag;
    private Gun gun;
    private Arm gun_arm;
    private Arm_pair.Handler_of_changing on_ammo_changed;
    
    public static Reload_pistol_simple create(
        Arm in_gun_arm,
        Gun in_gun,
        Baggage in_bag ,
        Arm_pair.Handler_of_changing on_ammo_changed
    ) {
        var action = object_pool.get<Reload_pistol_simple>();
        
        action.gun_arm = in_gun_arm;
        action.bag = in_bag;
        action.gun = in_gun;
        action.on_ammo_changed = on_ammo_changed;
        action.init_child_actions();
        
        return action;
    }


    private void init_child_actions() {
        add_children(
            Put_hand_before_bag.create(gun_arm, bag),
            Move_hand_into_bag.create(gun_arm, bag),
            Refill_gun.create(gun_arm, gun, bag, on_ammo_changed),
            Put_hand_before_bag.create(gun_arm, bag)
        );
        

    }

    protected override void on_start_execution() {
        base.on_start_execution();
        var reloadable = gun.GetComponent<Reloadable>();
        var audio_source = gun.GetComponent<AudioSource>();
        audio_source.PlayOneShot(reloadable.eject_magazine_sound);
    }


}
}