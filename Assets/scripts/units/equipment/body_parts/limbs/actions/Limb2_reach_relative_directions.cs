using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rvinowise.unity.extensions;

using rvinowise;
using rvinowise.unity.geometry2d;
using rvinowise.unity.units.parts.actions;
using rvinowise.unity.units.parts.limbs.arms;
using Action = rvinowise.unity.units.parts.actions.Action;

namespace rvinowise.unity.units.parts.limbs.actions {

public class Limb2_reach_relative_directions: Action_leaf {
    private Limb2 limb;

    Degree femur_rotation;
    Degree tibia_rotation;
    
    public static parts.actions.Action create_assuming_left_limb(
        Limb2 in_limb, 
        Degree in_femur_rotation,
        Degree in_tibia_rotation
    ) {
        
        var action = (Limb2_reach_relative_directions)pool.get(typeof(Limb2_reach_relative_directions));
        
        action.actor = in_limb;
        action.limb = in_limb;
        if (in_limb.side == Side.LEFT) {
            action.femur_rotation = in_femur_rotation;
            action.tibia_rotation = in_tibia_rotation;
        } else {
            action.femur_rotation = -in_femur_rotation;
            action.tibia_rotation = -in_tibia_rotation;
        } 
        
        return action;
    }

    public override void init_actors() {
        
        limb.segment1.set_target_direction_relative_to_parent(
            femur_rotation
        );
        limb.segment2.set_target_direction_relative_to_parent(
            tibia_rotation
        );
        
    }

    public override void restore_state() {
        base.restore_state();
        limb.segment1.target_direction_relative = false;
        limb.segment2.target_direction_relative = false;
    }

    public override void update() {
        if (complete()) {
            mark_as_completed();
        } else {
            limb.rotate_to_desired_directions();
        }
    }
  

    protected bool complete() {
        return limb.at_desired_rotation();
    }

}
}