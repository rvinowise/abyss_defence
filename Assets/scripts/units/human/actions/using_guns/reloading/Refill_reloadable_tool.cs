using System.Collections;
using UnityEngine;
using UnityEngine.Events;


namespace rvinowise.unity.actions {

public class Refill_reloadable_tool: Action_leaf {
    

    private Baggage bag;
    private Reloadable reloadable;
    private Arm gun_arm;
    private Arm_pair arm_pair;

    public float refilling_time = 0f; 
    public static Refill_reloadable_tool create(
        Arm_pair arm_pair,
        Arm gun_arm,
        Reloadable in_reloadable,
        Baggage in_bag
    ) {
        var action = object_pool.get<Refill_reloadable_tool>();
        action.add_actor(gun_arm);
        action.arm_pair = arm_pair;
        action.bag = in_bag;
        action.reloadable = in_reloadable;
        action.gun_arm = gun_arm;
        
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
        var audio_source = reloadable.GetComponent<AudioSource>();
        audio_source?.PlayOneShot(reloadable.insert_magazine_sound);
        
        yield return new WaitForSeconds(reloading_time);
        
        transfer_ammo_from_bag_to_gun();
        
        mark_as_completed();
    }

    private void transfer_ammo_from_bag_to_gun() {
        reloadable.ammo_qty = reloadable.max_ammo_qty;
        arm_pair.raise_on_ammo_changed(gun_arm,reloadable.ammo_qty);
    }
   


}
}