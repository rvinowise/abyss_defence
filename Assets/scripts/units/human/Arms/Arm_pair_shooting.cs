using rvinowise.unity.actions;
using rvinowise.unity.geometry2d;
using UnityEngine;


namespace rvinowise.unity {

public static class Arm_pair_shooting {

    public static bool IsInLayerMask(GameObject obj, LayerMask mask) => (mask.value & (1 << obj.layer)) != 0;
    public static bool IsInLayerMask(int layer, LayerMask mask) => (mask.value & (1 << layer)) != 0;
    
    public static LayerMask flying_obects = 
            LayerMask.GetMask("flying"); 
    
    public static bool is_target_flying(Transform target) {
        return IsInLayerMask(target.gameObject, flying_obects);
    } 
    
    
    public static void attack(Arm_pair arm_pair, Transform target, System.Action on_completed = null) {
        Debug.Log($"AIMING: Arm_pair.attack({target.name})");
        var arm = Arm_pair_aiming.get_arm_targeting(arm_pair, target);

        if (arm is null) {
            return;
        }
        
        IGun gun = null;
        if (target == Player_input.instance.cursor.transform) {
            fire_gun(arm_pair,arm, arm.get_held_gun());
        } else if (
            !arm_pair.is_on_cooldown()&&
            Arm.is_ready_to_attack_target(arm, target, ref gun)
        ) {
            if (is_target_flying(target)) {
                gun.set_vertical_pointing(IGun.Vertical_pointing.AIR);
            }
            else {
                gun.set_vertical_pointing(IGun.Vertical_pointing.GROUND);
            }
            fire_gun(arm_pair,arm, gun);
        }
    }
    
    public static void attack(Arm_pair arm_pair) {
        Debug.Log($"AIMING: Arm_pair.attack()");
        if (arm_pair.left_arm.get_held_gun() is {} gun) {
            gun.set_vertical_pointing(IGun.Vertical_pointing.GROUND);
            fire_gun(arm_pair,arm_pair.left_arm, gun);
        }
    }

    public static void fire_gun(Arm_pair arm_pair, Arm arm, IGun gun) {
        var reloadable = gun.get_reloadable();
        if (reloadable?.get_loaded_ammo() == 0) return;
        if (!arm_pair.is_arm_ready_to_fire(arm)) return;
        
        gun.pull_trigger();
        gun.release_trigger();
        arm_pair.last_shot_time = Time.time;
        
        if (reloadable) {
            arm_pair.raise_on_ammo_changed(arm, reloadable.get_loaded_ammo());

            if (reloadable.get_loaded_ammo() == 0) {
                //Debug.Log($"ATTACK: {gun.name} ammo_qty == 0, start reloading action");
                Action_sequential_parent.create(
                    Reload_pistol_simple.create(
                        arm_pair, arm, reloadable, arm_pair.baggage
                    )
                ).start_as_root(arm_pair.intelligence.action_runner);
            }
        }

    }
    
}

}