using System;
using System.Collections;
using System.Collections.Generic;
using geometry2d;
using UnityEngine;
using rvinowise;
using rvinowise.units.parts.limbs.arms.actions;
using rvinowise.units.parts.strategy;
using rvinowise.units.parts.weapons.guns;
using Action = rvinowise.units.parts.limbs.arms.actions.Action;


namespace rvinowise.units.parts.limbs.arms.strategy {

public class Idle_vigilant_main_arm: Action {

    /* parameters given by the user */
    private Transform target;
    private transport.ITransporter transporter; // movements of arms depend on where the body is moving
    
    /* inner parameters */
    private Quaternion upper_arm_offset_turn = Quaternion.identity;
    private Quaternion forearm_turn = Quaternion.identity;
    private Gun held_gun;
    
    
    
    public static Idle_vigilant_main_arm create(
        Action_tree in_action_tree, 
        Transform in_target,
        transport.ITransporter in_transporter
    ) {
        Idle_vigilant_main_arm action = (Idle_vigilant_main_arm)pool.get(typeof(Idle_vigilant_main_arm), in_action_tree);
        action.target = in_target;
        action.transporter = in_transporter;
        return action;
    }
    public Idle_vigilant_main_arm(Action_tree in_action_tree, Transform in_target): base(in_action_tree) {
        target = in_target;
    }
    
    public Idle_vigilant_main_arm() {
        
    }
    
    const float shoulder_thickness = 0.15f;
    private float distance_shoulder_to_wrist;
    public override void start() {
        if (arm.held_tool is Gun gun) {
            held_gun = gun;

            distance_shoulder_to_wrist = held_gun.stock_length - arm.hand.length + shoulder_thickness;
            upper_arm_offset_turn =
                arm.folding_direction.turn_quaternion(
                    geometry2d.Triangles.get_quaternion_by_lengths(
                        arm.upper_arm.length,
                        distance_shoulder_to_wrist,
                        arm.forearm.length
                    )
                );
        }
    }

    private Vector2 position_of_wrist;
    public override void update() {
        
        var direction_to_target = arm.upper_arm.transform.quaternion_to(target.position);

        var body_wants_to_turn = new Degree(
            transporter.command_batch.face_direction_degrees -    
            transporter.direction_quaternion.to_float_degrees()
        ).use_minus();
        
        arm.upper_arm.desired_direction = 
            determine_desired_direction_of_upper_arm(direction_to_target, body_wants_to_turn);


        
        arm.forearm.desired_direction = 
            determine_desired_direction_of_forearm(direction_to_target, body_wants_to_turn);


        arm.hand.desired_direction =
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
            Debug.Log("body wants LEFT");
        }

        return desired_direction;
    }

    
    private Quaternion determine_desired_direction_of_forearm(
        Quaternion direction_to_target, 
        Degree body_wants_to_turn
    ) {
        position_of_wrist = 
            arm.upper_arm.position + 
            (Vector2)(
                direction_to_target *
                (Vector2.right * distance_shoulder_to_wrist)
            );

        Quaternion direction_to_wrist =
            arm.upper_arm.desired_tip().quaternion_to(position_of_wrist);

        Quaternion desired_direction = direction_to_wrist;
        if (body_wants_to_turn.side() == Side.LEFT) {
            desired_direction = 
                direction_to_wrist * 
                body_wants_to_turn.to_quaternion().multiplied(1.1f).inverse();
        }

        return desired_direction;
    }

    
    public override void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(position_of_wrist,1f);
        Gizmos.DrawSphere(new Vector2(0f,0f),1f);
    }
}
}