using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using static UnityEngine.Input;
using geometry2d;
using rvinowise.units.control;
using rvinowise.units.parts;
using rvinowise.units.control;
using rvinowise.units.parts.limbs.arms;
using static geometry2d.Directions;

namespace rvinowise.units.control {

public class Player_control : Intelligence {

    private Transform transform;
    private Quaternion last_rotation;

    public Arm_controller arm_controller; //todo abstraction leak

    public Player_control(
        Transform in_transform,
        User_of_equipment in_user): base(in_user) 
    {
        transform = in_transform;
        //arm_controller = weaponry as Arm_controller;
    }

    public override void update() {
        read_input();
    }


    private void read_input() {
        read_transporter_input();
        read_tools_input();
        //read_switching_items_input();
        read_sensory_organs_input();
    }

    private void read_tools_input() {
        if (read_switching_items_input()) {
            is_switching_tool = true;
            foreach (Arm arm in arm_controller.arms) {
                arm.take_tool_from_baggage();
            }
        }
        if (!is_switching_tool) {
            foreach (Arm arm in arm_controller.arms) {
                idle(arm);
            }
        }
    }

    public bool is_switching_tool { get; set; } = false;

    private void idle(Arm arm) {
        var direction_to_mouse = transform.quaternion_to(rvi.Input.mouse_world_position());
        arm.upper_arm.desired_direction =
            arm.upper_arm.desired_idle_direction * direction_to_mouse;
        
        arm.forearm.desired_direction =
            arm.forearm.desired_idle_direction * direction_to_mouse;
        arm.hand.desired_direction =
            arm.hand.desired_idle_direction * direction_to_mouse;
    }

    private void read_sensory_organs_input() {
        sensory_organ?.pay_attention_to(rvi.Input.mouse_world_position());
    }

    private bool read_switching_items_input() {
        if (baggage == null) {
            return false;
        }
        int wheel_steps = rvi.Input.mouse_wheel_steps();
        if (Math.Abs(wheel_steps) > 0) {
        
            if (Directions.side(last_rotation) == Directions.Side.LEFT) {
                //baggage.switch_left_item(wheel_steps);
            }
            else {
                //baggage.switch_right_item(wheel_steps);
            }
            return true;
        }
        return false;
    }

    private void read_transporter_input() {
        if (transporter == null) {
            return;
        }
        transporter.command_batch.moving_direction_vector = read_moving_direction();
        transporter.command_batch.face_direction_quaternion = read_face_direction();

        Vector2 read_moving_direction() {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            Vector2 direction_vector = new Vector2(horizontal, vertical);
            return direction_vector.normalized;
        }

        Quaternion read_face_direction() {
            Vector2 mousePos = rvi.Input.mouse_world_position();
            Quaternion needed_direction = (mousePos - (Vector2) transform.position).to_quaternion();
            last_rotation = Quaternion.Inverse(needed_direction) * transform.rotation;
            return needed_direction;
        }
    }
}

}