using System;
using System.Collections;
using System.Collections.Generic;
using geometry2d;
using UnityEngine;
using rvinowise;
using rvinowise.units.parts.limbs;
using rvinowise.units.parts.limbs.arms;
//using Arm_controller = rvinowise.units.parts.limbs.Arms.humanoid.Arm_controller;


namespace rvinowise.units.human.init {
using rvinowise.units.parts.limbs.arms.humanoid;

public class Arms {

    private static readonly Vector2 scale = new Vector2(1f, 1f);
    private static Sprite sprite_upper_arm;
    private static Sprite sprite_forearm;

    public static Arm_controller init(Arm_controller controller) {
        sprite_upper_arm = Resources.Load<Sprite>("human/upper_arm");
        sprite_forearm = Resources.Load<Sprite>("human/forearm");
        
        IList<Arm> arms = create_arms(controller);

        init_common_parameters(controller);
        
        init_parameters_that_shoud_be_mirrored(controller);

        return controller;
    }

    private static IList<Arm> create_arms(Arm_controller controller) {
        for (int i = 0; i < 2; i++) {
            controller.add_tool(new Arm(controller.game_object.transform));
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
        arm.upper_arm.transform.parent = parent;
        
        arm.forearm.rotation_speed = 240f;
        arm.forearm.transform.parent = arm.upper_arm.transform;
        
        arm.hand.rotation_speed = 300f;
        arm.hand.transform.parent = arm.forearm.transform;
    }

    private static void init_parameters_that_shoud_be_mirrored(Arm_controller controller) {
        init_left_arm(
            controller.left_arm
        );
        mirror(controller.right_arm, controller.left_arm);
    }

    private static void init_left_arm(Arm arm) {
        arm.attachment = new Vector2(0f, 0.32f);
        
        arm.upper_arm.possible_span = new Span(100f, -50f);
        arm.upper_arm.tip = new Vector2(0.30f, 0f);
        arm.upper_arm.spriteRenderer.sprite = sprite_upper_arm;
        arm.upper_arm.local_position = new Vector2(0f, 0.32f);
        arm.upper_arm.desired_idle_direction = Directions.degrees_to_quaternion(20f);
        //arm.upper_arm.desired_idle_direction = Directions.degrees_to_quaternion(0f);
        
        arm.forearm.possible_span = new Span(0f, -160f);
        arm.forearm.tip = new Vector2(0.38f, 0f);
        arm.forearm.spriteRenderer.sprite = sprite_forearm;
        arm.forearm.local_position = arm.upper_arm.tip;
        arm.forearm.desired_idle_direction = Directions.degrees_to_quaternion(-20f);
        //arm.forearm.desired_idle_direction = Directions.degrees_to_quaternion(0f);
        
        arm.hand.possible_span = new Span(45f, -70f);
        arm.hand.tip = new Vector2(0.12f, 0f);
        arm.hand.spriteRenderer.sprite = Resources.Load<Sprite>("human/hand/grip_gun");
        arm.hand.local_position = arm.upper_arm.tip;
        arm.hand.desired_idle_direction = Directions.degrees_to_quaternion(0f);
    }

    private static void mirror(Arm arm_dst , Arm arm_src) {
        parts.limbs.init.Initializer.mirror(arm_dst, arm_src);
    }
}
}