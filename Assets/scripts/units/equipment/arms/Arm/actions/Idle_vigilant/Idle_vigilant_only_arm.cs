using System;
using System.Collections;
using System.Collections.Generic;
using rvinowise.unity.geometry2d;
using UnityEngine;
using rvinowise.unity.extensions;

using rvinowise;
using rvinowise.unity.units.parts.actions;
using rvinowise.unity.units.parts.limbs.arms.actions;

using rvinowise.unity.units.parts.weapons.guns;
using Action = rvinowise.unity.units.parts.actions.Action;


namespace rvinowise.unity.units.parts.limbs.arms.actions {

public class Idle_vigilant_only_arm: limbs.arms.actions.Action_of_arm {

    private Transform target;
    private transport.ITransporter transporter; // movements of arms depend on where the body is moving
    
    public static Idle_vigilant_only_arm create(
        Arm in_arm,
        Transform in_target,
        transport.ITransporter in_transporter
    ) {
        Idle_vigilant_only_arm action = 
            (Idle_vigilant_only_arm)pool.get(typeof(Idle_vigilant_only_arm));
        action.actor = in_arm;
        
        action.arm = in_arm;
        action.target = in_target;
        action.transporter = in_transporter;
        return action;
    }


    public override void init_state() {
        base.init_state();
       /*  arm.shoulder.set_target_direction_relative_to_parent(
            arm.shoulder.desired_idle_rotation
        ); */
        arm.shoulder.target_direction_relative = false;
        arm.upper_arm.target_direction_relative = false;
        arm.forearm.target_direction_relative = false;
        arm.hand.target_direction_relative = false;
    }
    
    
    const float controllable_velocity = 0.1f;
    public override void update() {
        
        var direction_to_target = arm.upper_arm.transform.quaternion_to(target.position);
        
        var body_wants_to_turn = new Degree(
            transporter.command_batch.face_direction_degrees -    
            transporter.direction_quaternion.to_float_degrees()
        ).use_minus();
        
        arm.shoulder.target_rotation = 
            arm.shoulder.desired_idle_rotation * direction_to_target;//transporter.direction_quaternion;
        
        arm.upper_arm.target_rotation =
            arm.upper_arm.desired_idle_rotation * direction_to_target;
        
        arm.forearm.target_rotation =
            arm.forearm.desired_idle_rotation * direction_to_target;
        
        arm.hand.target_rotation =
            arm.hand.desired_idle_rotation * direction_to_target;

        
        /* smooth movement with velocity for recoil */
        
        Vector2 last_hand_position = arm.hand.transform.position;

        arm.rotate_to_desired_directions();
       
    }
    
    private Quaternion determine_desired_direction_of_upper_arm(
        Quaternion direction_to_target, 
        Degree body_wants_to_turn
    ) {
        
        Quaternion desired_direction =
            direction_to_target * arm.upper_arm.desired_idle_rotation;

        if (body_wants_to_turn.side() == Side.LEFT) {
            desired_direction *= body_wants_to_turn.to_quaternion().multiplied(1.1f).inverse();
        }

        return desired_direction;
    }

    
    private Quaternion determine_desired_direction_of_forearm(
        Quaternion direction_to_target, 
        Degree body_wants_to_turn
    ) {

        Quaternion desired_direction = direction_to_target * arm.forearm.desired_idle_rotation;
        
        if (body_wants_to_turn.side() == Side.LEFT) {
            desired_direction = 
                desired_direction *
                body_wants_to_turn.to_quaternion().multiplied(1.1f).inverse();
        }

        return desired_direction;
    }
}
}