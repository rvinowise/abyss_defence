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
using Vector2 = System.Numerics.Vector2;


namespace rvinowise.unity.units.parts.limbs.arms.actions.using_guns.reloading {

public class Expose_gun_for_reloading_COMPLEX: parts.actions.Action_leaf {

    public Arm arm;

    //private Gun gun;
    private Pistol pistol;
    
    public static parts.actions.Action create(
        Arm in_arm
    ) {
        Contract.Requires(
            in_arm.held_tool is Gun,
            "the arm must hold a gun to reload it"
        );
        
        var action = (Expose_gun_for_reloading_COMPLEX)pool.get(typeof(Expose_gun_for_reloading_COMPLEX));
        action.arm = in_arm;
        
        if (in_arm.held_tool is Pistol pistol) {
            action.pistol = pistol;
        }
        
        return action;
    }
    
    public override void init_actors() {
        base.init_actors();
        arm.hand.gesture = Hand_gesture.Support_of_horizontal;
        arm.held_tool.transform.flipY(arm.folding_side == Side.RIGHT);
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