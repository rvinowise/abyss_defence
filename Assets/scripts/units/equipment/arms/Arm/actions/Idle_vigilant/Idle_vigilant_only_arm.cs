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

public class Idle_vigilant_only_arm: Action {

    private Transform target;
    private transport.ITransporter transporter; // movements of arms depend on where the body is moving
    
    public static Idle_vigilant_only_arm create(
        Action_tree in_action_tree,
        Transform in_target,
        transport.ITransporter in_transporter
    ) {
        Idle_vigilant_only_arm action = (Idle_vigilant_only_arm)pool.get(typeof(Idle_vigilant_only_arm), in_action_tree);
        action.target = in_target;
        action.transporter = in_transporter;
        return action;
    }
    public Idle_vigilant_only_arm(Action_tree in_action_tree, Transform in_target): base(in_action_tree) {
        target = in_target;
    }
    
    public Idle_vigilant_only_arm() {
        
    }
    
    public override void start() {
    }
    
    
    const float controllable_velocity = 0.1f;
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
        
        
        arm.upper_arm.desired_direction =
            arm.upper_arm.desired_idle_direction * direction_to_target;
        
        arm.forearm.desired_direction =
            arm.forearm.desired_idle_direction * direction_to_target;
        
        arm.hand.desired_direction =
            arm.hand.desired_idle_direction * direction_to_target;

        
        /* smooth movement with velocity for recoil */
        
        Vector2 last_hand_position = arm.hand.transform.position;
        //arm.jump_to_desired_directions();


        arm.rotate_to_desired_directions();
        Vector2 desired_hand_position = (Vector2) arm.hand.transform.position;
        Vector2 desired_hand_movement = desired_hand_position - last_hand_position;
        arm.hand.velocity += desired_hand_movement;
        Vector2 final_hand_position = last_hand_position + arm.hand.velocity;
        
        if (arm.hand.velocity.magnitude < controllable_velocity) {
            arm.hand.velocity = Vector2.zero;
        } else {
            float slowing_drag = (Desert_eagle.recoil_force - controllable_velocity) / 0.2f;
            arm.hand.velocity = arm.hand.velocity.shortened(slowing_drag * Time.deltaTime);
        }
        Debug.DrawLine(arm.hand.position,arm.hand.position+arm.hand.velocity*10f,Color.cyan);
        Debug.DrawLine(arm.hand.position,final_hand_position, Color.red);


        arm.hold_onto_point(final_hand_position);
    }
    
    private Quaternion determine_desired_direction_of_upper_arm(
        Quaternion direction_to_target, 
        Degree body_wants_to_turn
    ) {
        
        Quaternion desired_direction =
            direction_to_target * arm.upper_arm.desired_idle_direction;

        if (body_wants_to_turn.side() == Side.LEFT) {
            desired_direction *= body_wants_to_turn.to_quaternion().multiplied(1.1f).inverse();
        }

        return desired_direction;
    }

    
    private Quaternion determine_desired_direction_of_forearm(
        Quaternion direction_to_target, 
        Degree body_wants_to_turn
    ) {

        Quaternion desired_direction = direction_to_target * arm.forearm.desired_idle_direction;;
        
        if (body_wants_to_turn.side() == Side.LEFT) {
            desired_direction = 
                desired_direction *
                body_wants_to_turn.to_quaternion().multiplied(1.1f).inverse();
        }

        return desired_direction;
    }
}
}