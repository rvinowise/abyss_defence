using System;
using System.Collections;
using System.Collections.Generic;
using geometry2d;
using UnityEngine;
using rvinowise;
using rvinowise.units.parts;
using rvinowise.units.parts.limbs;
using rvinowise.units.parts.limbs.arms;
//using Arm_controller = rvinowise.units.parts.limbs.Arms.humanoid.Arm_controller;


namespace rvinowise.units.human.init {
using rvinowise.units.parts.limbs.arms.humanoid;

public class Arms {

    private static readonly Vector2 scale = new Vector2(1f, 1f);
    private static Sprite sprite_upper_arm;
    private static Sprite sprite_forearm;

    private static Baggage baggage;
    private static Transform idle_target;
    
    public static Arm_controller init(
        Arm_controller controller,
        Baggage in_baggage,
        Transform in_idle_target
    ) {
        sprite_upper_arm = Resources.Load<Sprite>("human/upper_arm");
        sprite_forearm = Resources.Load<Sprite>("human/forearm");
        baggage = in_baggage;
        idle_target = in_idle_target;
        
        IList<Arm> arms = create_arms(controller);

        init_common_parameters(controller);
        
        init_parameters_that_shoud_be_mirrored(controller);
        foreach (Arm arm in arms) {
            init_arm_parameters_that_can_be_inferred(arm);
        }
        init_parameters_that_can_be_inferred(controller);

        controller.arms.RemoveAt(0);
        
        return controller;
    }

    

    private static IList<Arm> create_arms(Arm_controller controller) {
        for (int i = 0; i < 2; i++) {
            controller.add_child(new Arm(
                controller,
                rvinowise.ui.input.Input.instance.cursor.center.transform
            ));
        }

        return controller.arms;
    }

    private static void init_common_parameters(Arm_controller controller) {
        foreach (Arm arm in controller.arms) {
            init_common_characteristic(arm, controller.transform);
        }
    }

    private const float rotation_speed = 240f;
    private static void init_common_characteristic(Arm arm, Transform parent) {
        arm.upper_arm.rotation_speed = 200f;
        arm.upper_arm.parent = parent;
        
        arm.forearm.rotation_speed = 240f;
        arm.forearm.parent = arm.upper_arm.transform;
        
        arm.hand.rotation_speed = 300f;
        arm.hand.parent = arm.forearm.transform;
        
        arm.baggage = baggage;
        arm.idle_target = idle_target;
    }

    private static void init_parameters_that_shoud_be_mirrored(Arm_controller controller) {
        init_left_arm(
            controller.left_arm
        );
        mirror(controller.right_arm, controller.left_arm);
    }

    private static void init_left_arm(Arm arm) {
        arm.local_position = new Vector2(0f, 0.32f);
        
        arm.upper_arm.possible_span = new Span(100f, -50f);
        arm.upper_arm.tip = new Vector2(0.30f, 0f);
        arm.upper_arm.local_position = new Vector2(0f, 0.32f);
        arm.upper_arm.desired_idle_direction = Directions.degrees_to_quaternion(20f);
        arm.upper_arm.spriteRenderer.sprite = sprite_upper_arm;
        
        arm.forearm.possible_span = new Span(0f, -150f);
        arm.forearm.tip = new Vector2(0.30f, 0f);
        arm.forearm.local_position = arm.upper_arm.tip;
        arm.forearm.desired_idle_direction = Directions.degrees_to_quaternion(-20f);
        arm.forearm.spriteRenderer.sprite = sprite_forearm;
        
        //arm.hand.possible_span = new Span(45f, -80f);
        arm.hand.possible_span = new Span(90f, -80f);
        
        arm.hand.local_position = arm.forearm.tip; //todo set localPosition automatically since it's always = parent.tip
        arm.hand.desired_idle_direction = Directions.degrees_to_quaternion(0f);
        //arm.folding_direction = geometry2d.Side.LEFT;
    }

    private static void mirror(Arm arm_dst , Arm arm_src) {
        parts.limbs.init.Initializer.mirror(arm_dst, arm_src);
    }

    private static void init_arm_parameters_that_can_be_inferred(Arm arm) {
        arm.folding_direction = -arm.segment2.possible_span.side_of_bigger_rotation();
    }
    private static void init_parameters_that_can_be_inferred(Arm_controller controller) {
        controller.shoulder_span =
            (controller.left_arm.local_position - controller.right_arm.local_position).
            magnitude;
    }
}
}