using System;
using System.Collections;
using System.Collections.Generic;
using rvinowise.unity.geometry2d;
using UnityEngine;
using rvinowise.unity.extensions;

using rvinowise;
using rvinowise.debug;
using rvinowise.contracts;
using rvinowise.unity.units.control.human;
using rvinowise.unity.units.parts.limbs;
using rvinowise.unity.units.parts.limbs.arms.actions;
using rvinowise.unity.units.parts.limbs.arms.actions.using_guns.reloading;
using rvinowise.unity.units.parts.actions;
using rvinowise.unity.units.parts.tools;
using rvinowise.unity.units.parts.transport;
using rvinowise.unity.units.parts.weapons.guns;
using Action = rvinowise.unity.units.parts.actions.Action;

namespace rvinowise.unity.units.parts.limbs.arms.humanoid {

public class Arm_controller: limbs.arms.Arm_controller {

    private units.humanoid.Humanoid user;
    public Arm left_arm {
        get { return arms[0]; }
        set { arms[0] = value; }
    }
    
    public Arm right_arm {
        get {
            return arms[1];
        }
        set { arms[1] = value; }
    }
    public float shoulder_span { get; set; }

    
    protected override void Awake() {
        base.Awake();
    }
    
    protected override void Start() {
        base.Start();
        init_directions();
        start_default_activity();
    }

    private void init_directions() {
        left_arm.shoulder.desired_idle_rotation = Directions.degrees_to_quaternion(90f);
        left_arm.upper_arm.desired_idle_rotation = Directions.degrees_to_quaternion(20f);
        left_arm.forearm.desired_idle_rotation = Directions.degrees_to_quaternion(-20f);
        left_arm.hand.desired_idle_rotation = Directions.degrees_to_quaternion(0f);

        right_arm.shoulder.desired_idle_rotation = Directions.degrees_to_quaternion(-90f);
        right_arm.upper_arm.desired_idle_rotation = Directions.degrees_to_quaternion(-20f);
        right_arm.forearm.desired_idle_rotation = Directions.degrees_to_quaternion(20f);
        right_arm.hand.desired_idle_rotation = Directions.degrees_to_quaternion(0f);
    }

    private void start_default_activity() {
        left_arm.start_idle_action();
        right_arm.start_idle_action();
    }

    public void reload(Arm gun_arm) {
        Contract.Requires(gun_arm.held_tool is Gun, "reloaded arm must hold a gun");

        if (is_reloading_now(gun_arm)) {
            //return;
        }
        
        Arm ammo_arm = other_arm(gun_arm);

        Action reloading_action = null;
        if (gun_arm.held_tool is Pistol pistol) {

            Ammunition magazine = user.baggage.retrieve_ammo_for_gun(pistol);
            Contract.Requires(magazine != null);
            
            reloading_action = Reload_pistol.create(
                user,
                gun_arm,
                ammo_arm,
                user.baggage,
                pistol,
                magazine
            );
        } else if (gun_arm.held_tool is Pump_shotgun shotgun) {
            reloading_action = Reload_shotgun.create(
                user,
                gun_arm,
                ammo_arm,
                user.baggage,
                shotgun,
                user.baggage.retrieve_ammo_for_gun(shotgun)
            );
        }
        
        
        user.set_root_action(
            Action_sequential_parent.create(
                reloading_action,
                Action_parallel_parent.create(
                    Idle_vigilant_only_arm.create(gun_arm,gun_arm.attention_target, transporter),
                    Idle_vigilant_only_arm.create(ammo_arm,gun_arm.attention_target, transporter)
                )
            )
        );

    }

    public bool is_reloading_now(Arm weapon_holder) {
        //Arm ammo_taker = other_arm(weapon_holder); 
        if (
            (user.current_action is Action_sequential_parent sequential_parent)&&
            (sequential_parent.current_child_action is Reload_pistol action_reload_pistol)&&
            (action_reload_pistol.gun_arm == weapon_holder)
        )
        {
            return true;
        }
        
        return false;
    }

    public Arm other_arm(Arm in_arm) {
        Contract.Requires(in_arm == left_arm || in_arm == right_arm, "arm should be mine");
        if (in_arm == left_arm) {
            return right_arm;
        }
        return left_arm;
    }
}
}