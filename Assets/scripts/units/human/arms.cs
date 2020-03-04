using System;
using System.Collections;
using System.Collections.Generic;
using geometry2d;
using UnityEngine;
using rvinowise;
using rvinowise.units.equipment.limbs;
using rvinowise.units.equipment.limbs.arms;
using units.equipment.arms.humanoid;


namespace rvinowise.units.human.init {

public class arms {

    private static readonly Vector2 scale = new Vector2(1f, 1f);
    private static Sprite sprite_upper_arm;
    private static Sprite sprite_forearm;

    public static void init(Arm_controller controller) {
        sprite_upper_arm = Resources.Load<Sprite>("human/upper_arm");
        sprite_forearm = Resources.Load<Sprite>("human/forearm");
        
        IList<Arm> arms = create_arms(controller);

        init_common_parameters(controller);
        
        init_parameters_that_shoud_be_mirrored(controller);


    }

    private static IList<Arm> create_arms(Arm_controller controller) {
        for (int i = 0; i < 2; i++) {
            controller.add_tool(new Arm(controller.game_object.transform));
        }

        return controller.arms;
    }

    private static void init_common_parameters(Arm_controller controller) {
        foreach (Arm arm in controller.arms) {
            init_common_characteristic(arm);
        }
    }

    private const float rotation_speed = 180f;
    private static void init_common_characteristic(Arm arm) {
        
        arm.upper_arm.tip = new Vector2(0.4225f, 0f);
        arm.upper_arm.spriteRenderer.sprite = sprite_upper_arm;
        arm.upper_arm.rotation_speed = rotation_speed;
//        arm.forearm.tip = new Vector2(0.5525f, 0f);
        arm.forearm.tip = new Vector2(0.56f, 0f);
        arm.forearm.spriteRenderer.sprite = sprite_forearm;
        arm.forearm.attachment = arm.upper_arm.tip;
        arm.forearm.rotation_speed = rotation_speed;
    }

    private static void init_parameters_that_shoud_be_mirrored(Arm_controller controller) {
        init_left_arm(
            controller.left_arm
        );
        mirror(controller.right_arm, controller.left_arm);
    }

    private static void init_left_arm(Arm arm) {
        arm.attachment = new Vector2(0f, 0.5f);
        arm.upper_arm.possible_span = new Span(100f, -70f);
        arm.forearm.possible_span = new Span(0f, 160f);
        /*arm.upper_arm.desired_relative_direction = Directions.degrees_to_quaternion(0f);
        arm.forearm.desired_relative_direction = Directions.degrees_to_quaternion(0f);*/
    }

    private static void mirror(Arm arm_dst , Arm arm_src) {
    }
}
}