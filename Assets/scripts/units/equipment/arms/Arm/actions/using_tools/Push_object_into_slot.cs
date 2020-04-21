using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rvinowise;
using geometry2d;
using rvinowise.units.parts.tools;
using units.equipment.arms.Arm.actions;
using units.equipment.parts.actions.Action;


namespace rvinowise.units.parts.limbs.arms.actions {

public class Push_object_into_slot: Arm_reach_orientation {
    public Arm arm;
    public Tool insertee;
    public Slot slot;
    private float old_rotation_speed;

    
    public static Push_object_into_slot create(
        Action_parent in_action_parent, 
        Arm in_arm,
        Tool in_insertee,
        Slot in_slot
    ) {
        var action = (Push_object_into_slot)pool.get(typeof(Push_object_into_slot), in_action_parent);
        action.arm = in_arm;
        action.insertee = in_insertee;
        action.slot = in_slot;
        return action;
    }
    public Push_object_into_slot() {}
    

    public override void init_state() {
        base.init_state();
        slow_movements(arm);
        desired_orientation = slot.get_orientation_inside();
    }

    public override void restore_state() {
        restore_movements(arm);
        base.restore_state();
    }
    
    private void slow_movements(Arm arm) {
        old_rotation_speed = arm.upper_arm.rotation_speed;
        arm.upper_arm.rotation_speed /= 2f;
        Debug.Log("slow_movements: "+arm.upper_arm.rotation_speed);
    }
    private void restore_movements(Arm arm) {
        arm.upper_arm.rotation_speed = old_rotation_speed;
        Debug.Log("restore_movements: "+arm.upper_arm.rotation_speed);
    }
    
    public override void update() {
        desired_orientation.adjust_to_parent();
        base.update();
    }
}
}