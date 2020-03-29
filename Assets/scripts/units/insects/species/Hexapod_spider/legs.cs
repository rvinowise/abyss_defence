using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using geometry2d;
using rvinowise.units.parts.limbs;
using rvinowise.units.parts.limbs.init;
using rvinowise.units.parts.limbs.legs;
using rvinowise.units.parts;

namespace rvinowise.units.hexapod_spider.init {


static class Legs {
    private static readonly float scale = 1.0236f * Settings.scale;
    private static Sprite sprite_femur;// = Resources.Load<Sprite>("sprites/basic_spider/femur.png");
    private static Sprite sprite_tibia;// = Resources.Load<Sprite>("sprites/basic_spider/tibia.png");*/

    public static void init(Leg_controller controller) {
        sprite_femur = Resources.Load<Sprite>("basic_spider/femur");
        sprite_tibia = Resources.Load<Sprite>("basic_spider/tibia");
        
        List<Leg> legs = create_legs(controller);
        
        init_common_parameters(controller);
        init_parameters_that_shoud_be_mirrored(controller);

        init_parameters_that_can_be_inferred(controller);
        
        create_moving_strategy(controller);
    }

    private static void create_moving_strategy(Leg_controller controller) {
        controller.moving_strategy = new parts.limbs.legs.strategy.Stable(controller.legs) {
            stable_leg_groups = new List<Stable_leg_group>() {
                new Stable_leg_group(
                    new List<Leg>() {
                        controller.left_front_leg,
                        controller.right_hind_leg
                    }
                ),
                new Stable_leg_group(
                    new List<Leg>() {
                        controller.legs[4],
                        controller.legs[5]
                    }
                ),
                new Stable_leg_group(
                    new List<Leg>() {
                        controller.right_front_leg,
                        controller.left_hind_leg
                    }
                )
            }
        };
    }

    private static void init_parameters_that_shoud_be_mirrored(Leg_controller controller) {
        init_left_front_leg(
            controller.left_front_leg
        );
        mirror(controller.right_front_leg, controller.left_front_leg);

        init_left_hind_leg(
            controller.left_hind_leg
        );
        mirror(controller.right_hind_leg, controller.left_hind_leg);

        init_left_middle_leg(
            controller.legs[4]);
        mirror(controller.legs[5], controller.legs[4]);
    }

    private static List<Leg> create_legs(Leg_controller controller) {
        for (int i = 0; i < 6; i++) {
            controller.add_tool(new Leg(controller.game_object.transform));
        }
        controller.left_front_leg.debug.name = "left_front_leg";
        controller.right_front_leg.debug.name = "right_front_leg";
        controller.left_hind_leg.debug.name = "left_hind_leg";
        controller.right_hind_leg.debug.name = "right_hind_leg";

        return controller.legs;
    }

    private const float rotation_speed = 360f;

    private static void init_left_front_leg(Leg leg) {
        leg.local_position = new Vector2(0.2275f, 0.325f)* scale;
        leg.femur.possible_span = new Span(0f, 170f);
        //leg.femur.comfortable_span = leg.femur.possible_span.scaled(0.5f);
        leg.tibia.possible_span = new Span(-170f, 0f);
        //leg.tibia.comfortable_span = leg.tibia.possible_span.scaled(0.5f);

        leg.femur.desired_relative_direction_standing = Directions.degrees_to_quaternion(45f);
        leg.tibia.desired_relative_direction_standing = Directions.degrees_to_quaternion(-70f);
    }
    
    private static void init_left_middle_leg(Leg leg) {
        leg.local_position = new Vector2(0f, 0.4225f) * scale;
        leg.femur.possible_span = new Span(20f, 160f);
        //leg.femur.comfortable_span = leg.femur.possible_span.scaled(0.5f);
        leg.tibia.possible_span = new Span(0f, 170f);
        //leg.tibia.comfortable_span = leg.tibia.possible_span.scaled(0.5f);
        leg.femur.desired_relative_direction_standing = Directions.degrees_to_quaternion(80f);
        leg.tibia.desired_relative_direction_standing = Directions.degrees_to_quaternion(40f);

    }

    private static void init_left_hind_leg(Leg leg) {
        leg.local_position = new Vector2(-0.2925f, 0.325f) * scale;
        leg.femur.possible_span = new Span(10f, 180f);
        //leg.femur.comfortable_span = leg.femur.possible_span.scaled(0.5f);
        leg.tibia.possible_span = new Span(0f, 170f);
        //leg.tibia.comfortable_span = leg.tibia.possible_span.scaled(0.5f);
        leg.femur.desired_relative_direction_standing = Directions.degrees_to_quaternion(150f);
        leg.tibia.desired_relative_direction_standing = Directions.degrees_to_quaternion(45f);
    }

    private static void init_common_characteristic(Leg leg) {
        leg.comfortable_distance = 0.6f * scale;
        leg.femur.tip = new Vector2(0.4225f, 0f) * scale;
        leg.femur.spriteRenderer.sprite = sprite_femur;
        leg.femur.rotation_speed = rotation_speed;
//        leg.tibia.tip = new Vector2(0.5525f, 0f);
        leg.tibia.tip = new Vector2(0.56f, 0f) * scale;
        leg.tibia.spriteRenderer.sprite = sprite_tibia;
        leg.tibia.local_position = leg.femur.tip;
        leg.tibia.rotation_speed = rotation_speed;
    }

    private static void init_parameters_that_can_be_inferred(Leg_controller controller) {
        foreach (Leg leg in controller.legs) {
            init_optimal_relative_position(leg);
            init_femur_folding_direction(leg);    
        }
    }


    private static void init_optimal_relative_position(Leg leg) {
        leg.optimal_relative_position_standing =
            leg.local_position +
            (Vector2) (leg.femur.desired_relative_direction_standing * leg.femur.tip) +
            (Vector2) (
                leg.tibia.desired_relative_direction_standing *
                leg.femur.desired_relative_direction_standing *
                leg.tibia.tip
            );
    }

    
    private static void init_common_parameters(Leg_controller controller) {
        foreach (Leg leg in controller.legs) {
            init_common_characteristic(leg);
        }
        controller.moving_offset_distance = 0.40f * scale;
    }
    
    private static void init_femur_folding_direction(Leg leg) {
        leg.folding_direction = -leg.tibia.possible_span.side_of_bigger_rotation();
    }

    private static void mirror(Leg dst, Leg src) {
        parts.limbs.init.Initializer.mirror(dst, src);
        
        dst.femur.desired_relative_direction_standing =
            Quaternion.Inverse(src.femur.desired_relative_direction_standing);
        dst.tibia.desired_relative_direction_standing =
            Quaternion.Inverse(src.tibia.desired_relative_direction_standing);

        copy_not_mirrored_leg_parameters(dst, src);
    }

    private static void copy_not_mirrored_leg_parameters(Leg dst, Leg src) {
        dst.tibia.local_position = src.femur.tip;
        dst.femur.rotation_speed = src.femur.rotation_speed;
        dst.tibia.rotation_speed = src.tibia.rotation_speed;
    }


}

}
