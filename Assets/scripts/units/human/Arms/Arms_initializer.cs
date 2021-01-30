using System;
using System.Collections;
using System.Collections.Generic;
using rvinowise.unity.geometry2d;
using UnityEngine;
using rvinowise.unity.extensions;

using rvinowise.unity.ui.input;
using rvinowise.unity.units.parts;
using rvinowise.unity.units.parts.limbs;
using rvinowise.unity.units.parts.limbs.arms;
using Segment = rvinowise.unity.units.parts.limbs.arms.Segment;

using Arm_controller = rvinowise.unity.units.parts.limbs.arms.humanoid.Arm_controller;
using rvinowise.unity.units.parts.limbs.arms.humanoid;


namespace rvinowise.unity.units.humanoid.init {

public class Arms_initializer: MonoBehaviour
{
    private static readonly Vector2 scale = new Vector2(1f, 1f);

    public Arm_controller arm_controller;
    public Baggage baggage;
    [HideInInspector]
    public Transform idle_target;
    
    private Arm left_arm;
    private Arm right_arm;


    
    public void Awake(
    ) {
        idle_target = rvinowise.unity.ui.input.Input.instance.cursor.transform;
        
        init_common_parameters(arm_controller);
        
        init_parameters_that_shoud_be_mirrored(arm_controller);
        foreach (Arm arm in arm_controller.arms) {
            init_arm_parameters_that_can_be_inferred(arm);
        }
        init_parameters_that_can_be_inferred(arm_controller);

    }

    

    private IList<Arm> create_arms(Arm_controller controller) {
        
        /*controller.left_arm.attach_tool_to_hand_for_holding(
            
        );*/

        return controller.arms;
    }

    private void init_common_parameters(Arm_controller controller) {
        foreach (Arm arm in controller.arms) {
            init_common_characteristic(arm, controller.transform);
        }
    }

    private const float rotation_speed = 240f;
    private void init_common_characteristic(Arm arm, Transform parent) {
        
        /*arm.shoulder.rotation_speed = 10f;
        arm.upper_arm.rotation_speed = 200f;
        arm.forearm.rotation_speed = 400f;
        arm.hand.rotation_speed = 300f;*/
        
        arm.shoulder.rotation_acceleration = 400f;
        arm.upper_arm.rotation_acceleration = 900f;
        arm.forearm.rotation_acceleration = 900f;
        arm.hand.rotation_acceleration = 1200f;

        arm.baggage = baggage;
        arm.attention_target = idle_target;
    }

    private void init_parameters_that_shoud_be_mirrored(Arm_controller controller) {
        init_left_arm(
            controller.left_arm
        );
        mirror(controller.right_arm, controller.left_arm);
    }

    private void init_left_arm(Arm arm) {
        
        arm.shoulder.possible_span = new Span(120f, 45f);
        arm.shoulder.init_length_to(arm.upper_arm);
        arm.shoulder.desired_idle_rotation = Directions.degrees_to_quaternion(90f);

        arm.upper_arm.possible_span = new Span(10f, -140f);
        arm.upper_arm.init_length_to(arm.forearm);
        arm.upper_arm.desired_idle_rotation = Directions.degrees_to_quaternion(20f);
        
        arm.forearm.possible_span = new Span(0f, -150f);
        arm.forearm.init_length_to(arm.hand);
        arm.forearm.desired_idle_rotation = Directions.degrees_to_quaternion(-20f);
        
        arm.hand.possible_span = new Span(20f, -80f);
        arm.hand.desired_idle_rotation = Directions.degrees_to_quaternion(0f);
    }

    private void mirror(Arm arm_dst , Arm arm_src) {
        arm_dst.shoulder.mirror_from(arm_src.shoulder);
        parts.limbs.init.Initializer.mirror(arm_dst, arm_src);
    }

    private void init_arm_parameters_that_can_be_inferred(Arm arm) {
        arm.folding_direction = -arm.segment2.possible_span.side_of_bigger_rotation();
    }
    private void init_parameters_that_can_be_inferred(Arm_controller controller) {
        controller.shoulder_span =
            (controller.left_arm.local_position - controller.right_arm.local_position).
            magnitude;
    }
}
}