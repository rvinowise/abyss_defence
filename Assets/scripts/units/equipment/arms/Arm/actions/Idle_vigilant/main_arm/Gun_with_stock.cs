using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rvinowise.unity.extensions;

using rvinowise;
using rvinowise.unity.geometry2d;
using rvinowise.unity.units.parts.actions;
using rvinowise.unity.units.parts.limbs.arms.actions;
using rvinowise.unity.units.parts.weapons.guns;


namespace rvinowise.unity.units.parts.limbs.arms.actions.idle_vigilant.main_arm {

public class Gun_with_stock: actions.Action_of_arm
{

    /* parameters given by the user */
    private Transform target;
    private transport.ITransporter transporter; // movements of arms depend on where the body is moving
    
    /* inner parameters */
    private Quaternion upper_arm_offset_turn = Quaternion.identity;
    private Quaternion forearm_turn = Quaternion.identity;
    private Gun held_gun;
    
    
    
    public static Gun_with_stock create(
        Action_sequential_parent in_action_sequential_parent,
        Transform in_target,
        transport.ITransporter in_transporter
    ) {
        Gun_with_stock action = (Gun_with_stock)pool.get(typeof(Gun_with_stock), in_action_sequential_parent);
        action.target = in_target;
        action.transporter = in_transporter;
        return action;
    }
    
    public Gun_with_stock() {
        
    }
    
    const float shoulder_thickness = 0.15f;
    private float distance_shoulder_to_wrist;
    public override void init_state() {
        base.init_state();
        if (arm.held_tool is Gun gun) {
            held_gun = gun;

            if (gun.has_stock) {
                distance_shoulder_to_wrist = held_gun.stock_length - arm.hand.absolute_length + shoulder_thickness;
            }
            else {
                distance_shoulder_to_wrist = arm.length/2f;
            }
            upper_arm_offset_turn =
                arm.folding_direction.turn_quaternion(
                    unity.geometry2d.Triangles.get_quaternion_by_lengths(
                        arm.upper_arm.absolute_length,
                        distance_shoulder_to_wrist,
                        arm.forearm.absolute_length
                    )
                );
        }
    }

    private Vector2 position_of_wrist;
    public override void update() {
        base.update();
        
        var direction_to_target = arm.upper_arm.transform.quaternion_to(target.position);

        var body_wants_to_turn = new Degree(
            transporter.command_batch.face_direction_degrees -    
            transporter.direction_quaternion.to_float_degrees()
        ).use_minus();
        
        arm.upper_arm.target_quaternion = 
            determine_desired_direction_of_upper_arm(direction_to_target, body_wants_to_turn);


        
        arm.forearm.target_quaternion = 
            determine_desired_direction_of_forearm(direction_to_target, body_wants_to_turn);


        arm.hand.target_quaternion =
            direction_to_target;
        
        arm.rotate_to_desired_directions();
    }

    
    private Quaternion determine_desired_direction_of_upper_arm(
        Quaternion direction_to_target, 
        Degree body_wants_to_turn
    ) {
        
         Quaternion desired_direction =
            direction_to_target * upper_arm_offset_turn;

        if (body_wants_to_turn.side() == Side.LEFT) {
            desired_direction *= body_wants_to_turn.to_quaternion().multiplied(1.1f).inverse();
        }

        return desired_direction;
    }

    
    private Quaternion determine_desired_direction_of_forearm(
        Quaternion direction_to_target, 
        Degree body_wants_to_turn
    ) {
        position_of_wrist = 
            arm.upper_arm.position + 
            (
                direction_to_target *
                (Vector2.right * distance_shoulder_to_wrist)
            );

        Quaternion direction_to_wrist =
            arm.upper_arm.get_desired_tip().quaternion_to(position_of_wrist);

        Quaternion desired_direction = direction_to_wrist;
        if (body_wants_to_turn.side() == Side.LEFT) {
            desired_direction = 
                direction_to_wrist * 
                body_wants_to_turn.to_quaternion().multiplied(1.1f).inverse();
        }

        return desired_direction;
    }

    
 

}
}