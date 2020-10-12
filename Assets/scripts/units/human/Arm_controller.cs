using System;
using System.Collections;
using System.Collections.Generic;
using geometry2d;
using UnityEngine;
using rvinowise;
using rvinowise.debug;
using rvinowise.rvi.contracts;
using rvinowise.units.control.human;
using rvinowise.units.parts.limbs;
using rvinowise.units.parts.limbs.arms.actions;
using rvinowise.units.parts.limbs.arms.actions.using_guns.reloading;
using rvinowise.units.parts.actions;
using rvinowise.units.parts.tools;
using rvinowise.units.parts.transport;
using rvinowise.units.parts.weapons.guns;
using Action = rvinowise.units.parts.actions.Action;

namespace rvinowise.units.parts.limbs.arms.humanoid {

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

    public Arm_controller(IChildren_groups_host in_user, ITransporter transporter) 
        : base(in_user, transporter) { }

    public Arm_controller(units.humanoid.Humanoid in_user, ITransporter transporter)
        : base(in_user.gameObject, transporter) {
        user = in_user;
    }
    
    
    
    public void reload(Arm gun_arm) {
        Contract.Requires(gun_arm.held_tool is Gun, "reloaded arm must hold a gun");

        if (is_reloading_now(gun_arm)) {
            //return;
        }
        Debug.Log("started reloading");
        
        Arm ammo_arm = other_arm(gun_arm);

        //human.animator.SetTrigger("reload_pistol");
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
                    Idle_vigilant_only_arm.create(gun_arm,gun_arm.idle_target, transporter),
                    Idle_vigilant_only_arm.create(ammo_arm,gun_arm.idle_target, transporter)
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