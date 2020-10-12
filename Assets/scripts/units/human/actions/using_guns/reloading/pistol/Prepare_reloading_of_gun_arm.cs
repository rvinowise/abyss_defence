using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rvinowise;
using geometry2d;
using rvinowise.rvi.contracts;
using rvinowise.units.parts.actions;
using rvinowise.units.parts.weapons.guns;


namespace rvinowise.units.parts.limbs.arms.actions.using_guns.reloading {

public class Prepare_reloading_of_gun_arm: Arm_reach_relative_directions {

    public Arm arm;

    public static parts.actions.Action create(
        Action_sequential_parent in_sequential_parent, 
        Arm in_arm
    ) {
        Contract.Requires(
            in_arm.held_tool is Gun,
            "the arm must hold a gun to reload it"
        );
        
        var action = (Expose_gun_for_reloading_COMPLEX)pool.get(typeof(Expose_gun_for_reloading_COMPLEX), in_sequential_parent);
        action.arm = in_arm;
        
        return action;
    }
    

}
}