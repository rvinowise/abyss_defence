using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rvinowise.unity.extensions;

//using static UnityEngine.Input;
using rvinowise.unity.geometry2d;
using rvinowise.debug;
using rvinowise.unity.units.control;
using rvinowise.unity.units.parts;
using rvinowise.unity.units.parts.limbs.arms;
using rvinowise.unity.units.parts.limbs.arms.actions;
using rvinowise.unity.units.parts.limbs.arms.actions.idle_vigilant;
using rvinowise.unity.units.parts.limbs.arms.actions.idle_vigilant.main_arm;
using rvinowise.unity.units.parts.weapons.guns;
using static rvinowise.unity.geometry2d.Directions;
using Input = rvinowise.unity.ui.input.Input;
using Arm_controller = rvinowise.unity.units.parts.limbs.arms.humanoid.Arm_controller;

namespace rvinowise.unity.units.control.human {

public class Player_human : Human {

    private int[] held_tool_index;

    protected override void read_input() {
        read_transporter_input();
        read_switching_items_input();
        read_sensory_organs_input();
        read_using_tools_input();
    }


    private void read_using_tools_input() {
        Arm selected_arm = get_selected_arm();
        if (selected_arm.held_tool is Gun gun) {
            use_gun(selected_arm, gun);
        }
    
        

    }

    private void use_gun(Arm arm, Gun gun) {
        bool wants_to_shoot = UnityEngine.Input.GetMouseButtonDown(0); //Input.instance.mouse_down();
        if (wants_to_shoot) {
            shoot();
        }
        else {

            bool wants_to_reload = Input.instance.button_presed("reload");
            if (wants_to_reload) {
                arm_controller.reload(arm);
            }
        }

    }

    private void shoot() {
        bool has_shot = false;
        if (arm_controller?.right_arm.held_tool is Gun right_gun) {
            if (right_gun.ready_to_fire()) {
                right_gun.pull_trigger();
                has_shot = true;
            }
        }
        if (has_shot) {
            return;
        }
        if (arm_controller?.left_arm.held_tool is Gun left_gun) {
            if (left_gun.ready_to_fire()) {
                left_gun.pull_trigger();
            }
        }
    }

    private void idle(Arm arm) {
        var direction_to_mouse = transform.quaternion_to(Input.instance.mouse_world_position);
        arm.upper_arm.target_quaternion =
            arm.upper_arm.desired_idle_direction * direction_to_mouse;
        
        arm.forearm.target_quaternion =
            arm.forearm.desired_idle_direction * direction_to_mouse;
        arm.hand.target_quaternion =
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

            Arm selected_arm = get_selected_arm();
            selected_arm.take_tool_from_baggage(
                baggage.items[0]    
            );
            return true;
        }
        return false;
    }

    public Arm get_selected_arm() {
        /* if (Side.from_degrees(last_rotation) == unity.geometry2d.Side.LEFT) {
            return arm_controller.left_arm;
        }
        return arm_controller.right_arm; */

        return arm_controller.right_arm;
    }

    private int get_desired_weapon_index() {
        int wheel_steps = Input.instance.mouse_wheel_steps;
        if (Math.Abs(wheel_steps) > 0) {
            
        }
        return 0;
    }

    /*private bool wants_to_switch_tool() {
        return (switching_items_is_possible() && )
    }*/
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
            if (direction_vector.magnitude > 0) {
                var test = true;
            }
            return direction_vector.normalized;
        }

        Quaternion read_face_direction() {
            Vector2 mousePos = Input.instance.cursor.center.transform.position;
            Quaternion needed_direction = (mousePos - (Vector2) transform.position).to_quaternion();
            if (has_gun_in_2hands(out var gun))
            {
                needed_direction *= get_additional_rotation_for_2hands_gun(gun); 
                    
            }
            save_last_rotation(needed_direction);

            return needed_direction;
        }
    }

    private bool has_gun_in_2hands(out Gun out_gun) {
        if (
            (arm_controller?.right_arm.current_action is Idle_vigilant_main_arm) &&
            (arm_controller?.right_arm.held_tool is Gun gun)
        ) {
            out_gun = gun;
            return true;
        }
        out_gun = null;
        return false;
    }

    private Quaternion get_additional_rotation_for_2hands_gun(Gun gun) {
        
        float body_rotation = 
            unity.geometry2d.Triangles.get_angle_by_lengths(
                arm_controller.shoulder_span,
                gun.butt_to_second_grip_distance,
                arm_controller.left_arm.length-arm_controller.left_arm.hand.absolute_length
            ) -90f;
        
        
        if (float.IsNaN(body_rotation)) {
            return Quaternion.identity;
        }
        
        return degrees_to_quaternion(body_rotation);
    }

    
}

}