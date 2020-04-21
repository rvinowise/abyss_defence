using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rvinowise;
using geometry2d;
using rvinowise.units.parts.actions;
using rvinowise.units.parts.weapons.guns;
using units.equipment.arms.Arm.actions;


namespace rvinowise.units.parts.limbs.arms.actions.using_guns.reloading {

public class Reload_pistol: Action_parent {

    private Arm gun_arm;
    private Arm magazine_arm;
    private Baggage bag;
    private Pistol gun;
    private Magazine magazine;
    
    public static Reload_pistol create(
        Action_parent in_action_sequence_builder,
        Arm in_gun_arm,
        Arm in_magazine_arm,
        Baggage in_bag, 
        Pistol in_tool,
        Magazine in_magazine
    ) {
        var action = (Reload_pistol)pool.get(typeof(Reload_pistol), in_action_sequence_builder);
        action.gun_arm = in_gun_arm;
        action.magazine_arm = in_magazine_arm;
        action.bag = in_bag;
        action.gun = in_tool;
        action.magazine = in_magazine;
        action.init_child_actions();
        
        return action;
    }

    public override void init_state() {
        base.init_state();
    }

    public void init_child_actions() {
        
        /*magazine_arm.action.current_child_action = actions.Take_tool_from_bag.create(
            magazine_arm.action, 
            magazine_arm, 
            bag, 
            magazine
        );*/
        
        /*magazine_arm.action.add_next_child = actions.Insert_object_into_slot.create(
            magazine_arm.action, 
            magazine_arm, 
            gun.magazine_slot
        );*/

        gun_arm.action.current_child_action = Arm_reach_relative_directions.create(
            gun_arm.action, 
            gun_arm,
            Directions.degrees_to_quaternion(45f),
            Directions.degrees_to_quaternion(90f),
            Directions.degrees_to_quaternion(0f)
        );

        //add_next_child = actions.Prepare_tool_for_inserting_object.create(this, gun_arm);

    }

    /*public override void update() {
        magazine_arm.action.current_child_action.update();
        gun_arm.action.current_child_action.update();
        if (complete()) {
            transition_to_next_action();
        }
    }*/

    private bool complete() {
        return (
                   magazine.transform.position -
                   gun.magazine_slot.transform.position
               ).magnitude < Mathf.Epsilon;
    }
}
}