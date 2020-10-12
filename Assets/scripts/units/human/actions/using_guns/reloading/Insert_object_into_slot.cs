using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rvinowise;
using geometry2d;
using rvinowise.units.parts.actions;
using rvinowise.units.parts.limbs.arms.actions.using_guns.reloading;
using rvinowise.units.parts.tools;
using units.equipment.parts.actions.Action;


namespace rvinowise.units.parts.limbs.arms.actions {

public class Insert_object_into_slot: Action_sequential_parent {

    public Arm arm;
    public Slot slot;
    private Tool insertee;
    

    public static parts.actions.Action create(
        Action_sequential_parent in_sequential_parent, 
        Arm in_magazine_arm, 
        Slot in_magazine_slot
    ) {
        var action = (Insert_object_into_slot)pool.get(typeof(Insert_object_into_slot), in_sequential_parent);
        action.arm = in_magazine_arm;
        action.slot = in_magazine_slot;
        action.init_child_actions();
        
        return action;
    }

    private void init_child_actions() {
        current_child_action_setter = Arm_reach_transform.create(
            this, arm, slot.transform
        );
        
        new_next_child = actions.Push_object_into_slot.create(
            this, arm, insertee, slot
        );
    }
}
}