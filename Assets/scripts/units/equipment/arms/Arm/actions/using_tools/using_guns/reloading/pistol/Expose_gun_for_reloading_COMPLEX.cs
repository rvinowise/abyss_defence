using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rvinowise;
using geometry2d;
using rvinowise.rvi.contracts;
using rvinowise.units.parts.weapons.guns;
using units.equipment.arms.Arm.actions;
using Vector2 = System.Numerics.Vector2;


namespace rvinowise.units.parts.limbs.arms.actions.using_guns.reloading {

public class Expose_gun_for_reloading_COMPLEX: parts.actions.Action {

    public Arm arm;

    //private Gun gun;
    private Pistol pistol;
    
    public static parts.actions.Action create(
        Action_parent in_parent, 
        Arm in_arm
    ) {
        Contract.Requires(
            in_arm.held_tool is Gun,
            "the arm must hold a gun to reload it"
        );
        
        var action = (Expose_gun_for_reloading_COMPLEX)pool.get(typeof(Expose_gun_for_reloading_COMPLEX), in_parent);
        action.arm = in_arm;
        
        if (in_arm.held_tool is Pistol pistol) {
            action.pistol = pistol;
        }
        
        return action;
    }
    
    public override void init_state() {
        base.init_state();
        arm.hand.gesture = Hand_gesture.Support_of_horizontal;
        arm.held_tool.transform.flipY(arm.folding_direction == Side.RIGHT);
        arm.held_tool.animator.SetBool("sideview", true);
    }

    public override void restore_state() {
        base.restore_state();
        arm.held_tool.transform.flipY(false);
        arm.held_tool.animator.SetBool("sideview", false);
    }

    public override void update() {
        base.update();
        Debug.DrawLine(
            pistol.magazine_slot.transform.position,
            
            pistol.magazine_slot.transform.position + 
            pistol.magazine_slot.transform.TransformPoint(
                Vector3.right * pistol.magazine_slot.depth
            ),
            
            Color.magenta
        );
    }
}
}