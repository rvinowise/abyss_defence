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
using rvinowise.units.parts.transport;
using rvinowise.units.parts.weapons.guns;

namespace rvinowise.units.parts.limbs.arms.humanoid {

public class Arm_controller: limbs.arms.Arm_controller {

    private units.human.Human user;
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

    public Arm_controller(units.human.Human in_user, ITransporter transporter)
        : base(in_user.gameObject, transporter) {
        user = in_user;
    }
    
    
    
    public void reload(Arm gun_arm) {
        Contract.Requires(gun_arm.held_tool is Gun, "reloaded arm must hold a gun");

        if (is_reloading_now(gun_arm)) {
            return;
        }
        Log.info("started reloading");
        
        Arm magazine_arm = other_arm(gun_arm);

        //human.animator.SetTrigger("reload_pistol");
        
        if (gun_arm.held_tool is Pistol pistol) {

            magazine_arm.action.current_child_action_setter = actions.Take_tool_from_bag.create(
                magazine_arm.action, 
                magazine_arm, 
                magazine_arm.baggage,
                gun_arm.baggage.retrieve_ammo_for_gun(pistol) as Magazine
            );
            /*magazine_arm.action.new_next_child = actions.Idle_vigilant_only_arm.create(
                magazine_arm.action,
                magazine_arm,
                magazine_arm.idle_target, 
                transporter
            );*/


            gun_arm.action.current_child_action_setter = Arm_reach_relative_directions.create(
                gun_arm.action, 
                gun_arm,
                Directions.degrees_to_quaternion(-45+90f),
                Directions.degrees_to_quaternion(90f),
                Directions.degrees_to_quaternion(5f)
            );
            /*gun_arm.action.new_next_child = actions.Idle_vigilant_only_arm.create(
                gun_arm.action,
                gun_arm,
                gun_arm.idle_target, 
                transporter
            );*/
                        
        }


    }

    public bool is_reloading_now(Arm weapon_holder) {
        //Arm ammo_taker = other_arm(weapon_holder); 
        if (weapon_holder.action.current_child_action.GetType() == typeof(Arm_reach_relative_directions)) {
            return true;
        }
        return false;
    }

    private Arm other_arm(Arm in_arm) {
        Contract.Requires(in_arm == left_arm || in_arm == right_arm, "arm should be mine");
        if (in_arm == left_arm) {
            return right_arm;
        }
        return left_arm;
    }
}
}