﻿using System;
using rvinowise.debug;
using rvinowise.unity.units.parts.actions;
using rvinowise.unity.units.parts.tools;
using UnityEngine;
using rvinowise.unity.extensions;
using rvinowise.unity.geometry2d;
using rvinowise.unity.units.parts.transport;


namespace rvinowise.unity.units.parts.limbs.arms.actions {

public class Hitting_with_limb2: Action_sequential_parent {

    private IActor actor;
    public Limb2 limb;
    private ITransporter transporter;
    private Transform target;
    
    public static Hitting_with_limb2 create(
        Limb2 actor,
        ITransporter in_transporter,
        Transform in_target
    ) {
        var action = (Hitting_with_limb2)pool.get(typeof(Hitting_with_limb2));
        action.actor = actor;
        action.limb = actor;

        action.target = in_target;
        action.transporter = in_transporter;
        
        return action;
    }
    
    public override void init_children() {
        
        Hitting_with_limb2_swing_back swinging_subaction 
            = Hitting_with_limb2_swing_back.create(limb, transporter, target);
        Hitting_with_limb2_impact impacting_subaction
            = Hitting_with_limb2_impact.create(limb, transporter, target);
        swinging_subaction.limb = limb;
        impacting_subaction.limb = limb;
        Side swing_direction = find_swing_direction();
        swinging_subaction.swing_side = swing_direction;
        impacting_subaction.impact_side = -swing_direction;
        
        add_children(
            swinging_subaction,
            impacting_subaction
        );
    }
    private Side find_swing_direction() {
        var dir_to_target = get_direction_to_target();
        var segment1_current_swing = Mathf.DeltaAngle(
            dir_to_target,
            limb.segment1.transform.get_degrees()
            
        );
        var segment2_current_swing = Mathf.DeltaAngle(
            dir_to_target,
            limb.segment2.transform.get_degrees()
            
        );
        return Side.from_degrees(segment1_current_swing + segment2_current_swing);
    }
    private float get_direction_to_target() {
        return limb.transform.degrees_to(target.position);
    }


    
}
}