using System.Collections;
using UnityEngine;
using UnityEngine.Events;


namespace rvinowise.unity.actions {

public class Refill_gun: Action_leaf {
    

    private Baggage bag;
    private Gun gun;
    private Arm gun_arm;
    private Arm_pair.Handler_of_changing on_ammo_changed;

    public float refilling_time = 5; 
    public static Refill_gun create(
        Arm gun_arm,
        Gun in_gun,
        Baggage in_bag,
        Arm_pair.Handler_of_changing on_ammo_changed
    ) {
        var action = object_pool.get<Refill_gun>();
        
        action.bag = in_bag;
        action.gun = in_gun;
        action.gun_arm = gun_arm;
        action.on_ammo_changed = on_ammo_changed;
        
        return action;
    }

    protected override void on_start_execution() {
        base.on_start_execution();
        gun_arm.StartCoroutine(reloading_process(refilling_time));
    }

    public override void update() {
        base.update();
        
    }

    private IEnumerator reloading_process(float reloading_time) {
        var reloadable = gun.GetComponent<Reloadable>();
        var audio_source = gun.GetComponent<AudioSource>();
        audio_source.PlayOneShot(reloadable.insert_magazine_sound);
        
        yield return new WaitForSeconds(reloading_time);
        
        transfer_ammo_from_bag_to_gun();
        
        mark_as_completed();
    }

    private void transfer_ammo_from_bag_to_gun() {
        gun.ammo_qty = gun.max_ammo_qty;
        on_ammo_changed(gun_arm,gun.ammo_qty);
    }
   


}
}