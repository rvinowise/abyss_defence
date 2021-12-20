using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rvinowise.unity.extensions;

using rvinowise;
using rvinowise.unity.geometry2d;
using rvinowise.unity.units.parts.actions;
using rvinowise.unity.units.parts.tools;
using units.equipment.parts.actions.Action;


namespace rvinowise.unity.units.parts.limbs.arms.actions {

public class Push_object_into_slot: Arm_reach_orientation {
    public Tool insertee;
    public Slot slot;
    private float old_rotation_speed;

    
    public static Push_object_into_slot create(
        Arm in_arm,
        Tool in_insertee,
        Slot in_slot
    ) {
        var action = (Push_object_into_slot)pool.get(typeof(Push_object_into_slot));
        action.arm = in_arm;
        action.insertee = in_insertee;
        action.slot = in_slot;
        return action;
    }
    

    public override void init_actors() {
        base.init_actors();
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
    }
    private void restore_movements(Arm arm) {
        arm.upper_arm.rotation_speed = old_rotation_speed;
    }
    
    public override void update() {
        desired_orientation.adjust_to_parent();
        base.update();
    }
}
}