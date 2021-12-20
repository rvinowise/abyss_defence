using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rvinowise.unity.extensions;

using rvinowise;
using rvinowise.unity.geometry2d;
using rvinowise.unity.units.humanoid;
using rvinowise.unity.units.parts.actions;
using rvinowise.unity.units.parts.tools;
using rvinowise.unity.units.parts.weapons.guns;
using units.human.actions;
using units;

namespace rvinowise.unity.units.parts.limbs.arms.actions.using_guns.reloading {

public class Reload_shotgun: Reload_gun {
    
    private static readonly int animation_reloading = Animator.StringToHash("reload_shotgun");    

    private Baggage bag;
    private Pump_shotgun gun;
    private Ammunition ammo;
    

    public static Reload_shotgun create(
        Animator in_animator,
        Arm in_gun_arm,
        Arm in_magazine_arm,
        Baggage in_bag, 
        Pump_shotgun in_tool,
        Ammunition in_ammo
    ) {
        var action = (Reload_shotgun)pool.get(typeof(Reload_shotgun));
        action.gun_arm = in_gun_arm;
        action.ammo_arm = in_magazine_arm;
        action.bag = in_bag;
        action.gun = in_tool;
        action.ammo = in_ammo;
        action.init_child_actions();
        
        return action;
    }
    
    public static Reload_shotgun create(
        Humanoid in_body,
        Arm in_gun_arm,
        Arm in_magazine_arm,
        Baggage in_bag, 
        Pump_shotgun in_tool,
        Ammunition in_ammo
    ) {
        var action = (Reload_shotgun)pool.get(typeof(Reload_shotgun));
        
        action.animator = in_body.animator;
        action.body = in_body;
        action.gun_arm = in_gun_arm;
        action.ammo_arm = in_magazine_arm;
        action.bag = in_bag;
        action.gun = in_tool;
        action.ammo = in_ammo;
        action.init_child_actions();
        
        return action;
    }



    public void init_child_actions() {
        
        this.add_child(
            rvinowise.unity.units.parts.actions.Action_parallel_parent.create(
                actions.Arm_reach_relative_directions.create_assuming_left_arm(
                    gun_arm,
                    -90f,
                    -135f,
                    -90f,
                    0f
                ),
                actions.Take_tool_from_bag.create(
                    ammo_arm,
                    bag,
                    ammo
                )
            )
        );
        this.add_child(
            Start_recorded_animation.create(
                animator,
                animation_reloading
            )
        );
        

    }



    
}
}