using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rvinowise.unity.extensions;

using rvinowise;
using rvinowise.unity.geometry2d;
using rvinowise.contracts;
using rvinowise.unity.units.parts.actions;
using rvinowise.unity.units.parts.weapons.guns;


namespace rvinowise.unity.units.parts.limbs.arms.actions.using_guns.reloading {

public class Prepare_reloading_of_gun_arm: Arm_reach_relative_directions {

    public Arm arm;

    public static parts.actions.Action create(
        Arm in_arm
    ) {
        Contract.Requires(
            in_arm.held_tool is Gun,
            "the arm must hold a gun to reload it"
        );
        
        var action = (Expose_gun_for_reloading_COMPLEX)pool.get(typeof(Expose_gun_for_reloading_COMPLEX));
        action.arm = in_arm;
        
        return action;
    }
    

}
}