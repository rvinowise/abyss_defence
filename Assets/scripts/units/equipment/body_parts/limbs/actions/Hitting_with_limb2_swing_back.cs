﻿using System;
using System.Collections;
using System.Collections.Generic;
using rvinowise.unity.geometry2d;
using UnityEngine;
using rvinowise.unity.extensions;

using rvinowise;
using rvinowise.unity.units.parts.actions;
using rvinowise.unity.units.parts.limbs.arms.actions;
using rvinowise.unity.units.parts.transport;
using Action = rvinowise.unity.units.parts.actions.Action;


namespace rvinowise.unity.units.parts.limbs.arms.actions {

public class Hitting_with_limb2_swing_back: Action_leaf {

    public Limb2 limb{ set; get; }
    private ITransporter transporter;
    private Transform target;

    private float swing_direction = 180f;
    public Side swing_side { set; get; } = Side.NONE;

    public static Hitting_with_limb2_swing_back create(
        Limb2 in_limb,
        ITransporter in_transporter,
        Transform in_target
    ) {
        var action = (Hitting_with_limb2_swing_back)pool.get(typeof(Hitting_with_limb2_swing_back));
        action.actor = in_limb;
        
        action.limb = in_limb;
        action.target = in_target;
        action.transporter = in_transporter;
        return action;
    }
    public Hitting_with_limb2_swing_back() {
        
    }
    

    public override void init_actors() {
        base.init_actors();
        swing_side = find_swing_direction();
        slow_movements();
    }

    public override void restore_state() {
        base.restore_state();
        restore_movements();
    }

    private void slow_movements() {
        limb.segment1.rotation_speed /= 2f;
        limb.segment1.rotation_speed /= 2f;
    }
    private void restore_movements() {
        limb.segment1.rotation_speed *= 2f;
        limb.segment1.rotation_speed *= 2f;
    }


    public override void update() {
        base.update();
        //limb.move_segments_towards_desired_direction();

        limb.segment1.change_rotation_speed(swing_side, float.MaxValue);
        limb.segment2.change_rotation_speed(swing_side, float.MaxValue);
        limb.segment1.update_rotation();
        limb.segment2.update_rotation();
        transporter.command_batch.face_direction_degrees = get_direction_to_target() + swing_side.turn_degrees(45);
        keep_optimal_distance_from_target();
        if (complete()) {
            mark_as_completed();
        } else {
            mark_as_not_completed();
        }
    }

    private void keep_optimal_distance_from_target() {
        float distance_to_target = limb.transform.distance_to(target.position);
        float reaching_distance = limb.get_reaching_distance();

        if (distance_to_target > reaching_distance) {
            transporter.command_batch.moving_direction_vector = 
                (target.position - limb.transform.position).normalized;
        } else if (distance_to_target < reaching_distance * 0.9) {
            transporter.command_batch.moving_direction_vector = 
                (limb.transform.position - target.position).normalized;
        }
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
    

    protected bool complete() {
        if (
            direction_to_target_is_big_enough()
            ) 
        {
            return true;
        }
        return false;
    }

    private bool direction_to_target_is_big_enough() {
        var dir_to_target = get_direction_to_target();
        var segment1_swing = Mathf.DeltaAngle(
            limb.segment1.transform.get_degrees(),
            dir_to_target
        );
        var segment2_swing = Mathf.DeltaAngle(
            limb.segment2.transform.get_degrees(),
            dir_to_target
        );
        return Mathf.Abs(segment1_swing) + Mathf.Abs(segment2_swing) >= swing_direction;
    }
    
    private float get_direction_to_target() {
        return limb.transform.degrees_to(target.position);
    }

    
}
}