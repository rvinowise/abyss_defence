using System;
using rvinowise.unity.geometry2d;
using UnityEngine;
using rvinowise.unity.extensions;
using rvinowise.contracts;

namespace rvinowise.unity.actions {

public class Fire_gun_at_target: Aim_at_target {

    public IGun gun;
    
    public static Fire_gun_at_target create(
        IGun in_gun,
        Arm in_arm,
        Transform in_target,
        Transform body
    ) {
        

        var action = 
            (Fire_gun_at_target)object_pool.get(typeof(Fire_gun_at_target));
        action.add_actor(in_arm);

        action.gun = in_gun;
        action.arm = in_arm;
        action.target = in_target;
        action.body = body;
        return action;
    }
    

    protected override void on_start_execution() {
        Debug.Log($"AIMING: ({arm.name})Activate_supertool_at_target.on_start_execution({target.name})");
        //Contract.Assume(arm.get_held_gun() != null, "aiming arm should hold a gun");
        base.on_start_execution();
        //tool_description.prepare_for_taking();
    }


    public override void update() {
        base.update();
        if (is_ready_to_attack_target(arm, target.position)) {
            gun.pull_trigger();
            gun.release_trigger();
        }
        // if (gun.get_loaded_ammo() == 0) {
        //     mark_as_completed();
        // }
    }

    public bool is_ready_to_attack_target(Arm arm, Vector3 in_target) {
        var tool = arm.held_tool;
        var ready =
            arm &&
            tool &&
            //(gun.get_loaded_ammo() > 0) &&
            tool.is_aimed_at_point(in_target);

        return ready;
    }

}
}