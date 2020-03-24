using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using static UnityEngine.Input;
using geometry2d;
using rvinowise.units.control;
using rvinowise.units.parts;
using rvinowise.units.parts.limbs.arms;
using static geometry2d.Directions;
using Input = rvinowise.ui.input.Input;

namespace rvinowise.units.control.human {

public class Player : Human {

    private Transform transform;
    
    private float last_rotation;
    private int[] held_tool_index;

    public rvinowise.units.parts.limbs.arms.humanoid.Arm_controller arm_controller; //todo abstraction leak

    public Player(
        Transform in_transform,
        User_of_equipment in_user): base(in_user) 
    {
        transform = in_transform;
        //arm_controller = weaponry as Arm_controller;
    }

    public override void update() {
        read_input();
    }


    protected override void read_input() {
        read_transporter_input();
        read_switching_items_input();
        //read_switching_items_input();
        read_sensory_organs_input();
    }

    public bool is_switching_tool { get; set; } = false;

    /*private void read_tools_input() {
        if (read_switching_items_input()) {
            foreach (Arm arm in arm_controller.arms) {
                arm.take_tool_from_baggage();
            }
        }
       
    }*/

    private void idle(Arm arm) {
        var direction_to_mouse = transform.quaternion_to(Input.instance.mouse_world_position);
        arm.upper_arm.desired_direction =
            arm.upper_arm.desired_idle_direction * direction_to_mouse;
        
        arm.forearm.desired_direction =
            arm.forearm.desired_idle_direction * direction_to_mouse;
        arm.hand.desired_direction =
            arm.hand.desired_idle_direction * direction_to_mouse;
    }

    private void read_sensory_organs_input() {
        sensory_organ?.pay_attention_to(Input.instance.mouse_world_position);
    }

    private bool read_switching_items_input() {
        if (!switching_items_is_possible()) {
            return false;
        }
        int tool_index = get_desired_weapon_index();
        
        int wheel_steps = Input.instance.mouse_wheel_steps;
        if (Math.Abs(wheel_steps) > 0) {
        
            if (Side.from_degrees(last_rotation) == geometry2d.Side.LEFT) {
                arm_controller.left_arm.support_held_tool(
                    baggage.items[0]
                );
            }
            else {
                arm_controller.right_arm.take_tool_from_baggage(
                    baggage.items[0]    
                );

            }
            return true;
        }
        return false;
    }

    private int get_desired_weapon_index() {
        int wheel_steps = Input.instance.mouse_wheel_steps;
        if (Math.Abs(wheel_steps) > 0) {
            
        }
        return 0;
    }

    private bool switching_items_is_possible() {
        if (baggage == null) {
            return false;
        }
        if (Input.instance.zoom_held) {
            return false;
        }
        return true;
    }

    private void read_transporter_input() {
        if (transporter == null) {
            return;
        }
        transporter.command_batch.moving_direction_vector = read_moving_direction();
        transporter.command_batch.face_direction_quaternion = read_face_direction();

        Vector2 read_moving_direction() {
            Vector2 direction_vector = Input.instance.moving_vector;
            return direction_vector.normalized;
        }

        Quaternion read_face_direction() {
            Vector2 mousePos = Input.instance.mouse_world_position;
            Quaternion needed_direction = (mousePos - (Vector2) transform.position).to_quaternion();
            float angle_difference = transform.rotation.degrees_to(needed_direction);
            if (Mathf.Abs(angle_difference) > (float)Mathf.Epsilon) {
                last_rotation = angle_difference;
            }
    
            return needed_direction;
        }
    }
}

}