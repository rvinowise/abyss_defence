using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using geometry2d;



namespace rvinowise.units.hexapod_spider.init {
using rvinowise.units.equipment.limbs;
using units;

static class Legs {
    private static readonly Vector2 scale = new Vector2(0.65f, 0.65f);
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
        controller.moving_strategy = new equipment.limbs.strategy.Stable(controller.legs) {
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
            controller.left_front_leg,
            sprite_femur,
            sprite_tibia
        );
        mirror(controller.right_front_leg, controller.left_front_leg);

        init_left_hind_leg(
            controller.left_hind_leg,
            sprite_femur,
            sprite_tibia
        );
        mirror(controller.right_hind_leg, controller.left_hind_leg);

        init_left_middle_leg(
            controller.legs[4],
            sprite_femur,
            sprite_tibia);
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

    private const float rotation_speed = 8f;

    private static void init_left_front_leg(Leg leg, Sprite sprite_femur, Sprite sprite_tibia) {
        leg.attachment = new Vector2(0.2275f, 0.325f);
        leg.femur.possible_span = new Span(0f, 170f);
        //leg.femur.comfortable_span = leg.femur.possible_span.scaled(0.5f);
        leg.tibia.possible_span = new Span(-170f, 0f);
        //leg.tibia.comfortable_span = leg.tibia.possible_span.scaled(0.5f);

        leg.femur.desired_relative_direction_standing = Directions.degrees_to_quaternion(45f);
        leg.tibia.desired_relative_direction_standing = Directions.degrees_to_quaternion(-70f);
    }
    
    private static void init_left_middle_leg(Leg leg, Sprite sprite_femur, Sprite sprite_tibia) {
        leg.attachment = new Vector2(0f, 0.65f) * scale;
        leg.femur.possible_span = new Span(20f, 160f);
        //leg.femur.comfortable_span = leg.femur.possible_span.scaled(0.5f);
        leg.tibia.possible_span = new Span(0f, 170f);
        //leg.tibia.comfortable_span = leg.tibia.possible_span.scaled(0.5f);
        leg.femur.desired_relative_direction_standing = Directions.degrees_to_quaternion(80f);
        leg.tibia.desired_relative_direction_standing = Directions.degrees_to_quaternion(40f);

    }

    private static void init_left_hind_leg(Leg leg, Sprite sprite_femur, Sprite sprite_tibia) {
        leg.attachment = new Vector2(-0.45f, 0.5f) * scale;
        leg.femur.possible_span = new Span(10f, 180f);
        //leg.femur.comfortable_span = leg.femur.possible_span.scaled(0.5f);
        leg.tibia.possible_span = new Span(0f, 170f);
        //leg.tibia.comfortable_span = leg.tibia.possible_span.scaled(0.5f);
        leg.femur.desired_relative_direction_standing = Directions.degrees_to_quaternion(150f);
        leg.tibia.desired_relative_direction_standing = Directions.degrees_to_quaternion(45f);
    }

    private static void init_common_characteristic(Leg leg) {
        leg.comfortable_distance = 0.6f;
        leg.femur.tip = new Vector2(0.4225f, 0f);
        leg.femur.spriteRenderer.sprite = sprite_femur;
        leg.femur.rotation_speed = rotation_speed;
//        leg.tibia.tip = new Vector2(0.5525f, 0f);
        leg.tibia.tip = new Vector2(0.56f, 0f);
        leg.tibia.spriteRenderer.sprite = sprite_tibia;
        leg.tibia.attachment_point = leg.femur.tip;
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
            leg.attachment +
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
        controller.moving_offset_distance = 0.40f;
    }
    
    private static void init_femur_folding_direction(Leg leg) {
        leg.femur_folding_direction = -1 * leg.tibia.possible_span.sign_of_bigger_rotation();
    }

    private static void mirror(Leg dst, Leg src) {
        // the base direction is to the right
        dst.attachment = new Vector2(
            src.attachment.x,
            -src.attachment.y
        );
        dst.femur.possible_span = mirror_span(src.femur.possible_span);
        //dst.femur.comfortable_span = mirror_span(src.femur.comfortable_span);
        dst.femur.tip = new Vector2(
            src.femur.tip.x,
            -src.femur.tip.y
        );

        dst.tibia.possible_span = mirror_span(src.tibia.possible_span);
        //dst.tibia.comfortable_span = mirror_span(src.tibia.comfortable_span);
        dst.tibia.tip = new Vector2(
            src.tibia.tip.x,
            -src.tibia.tip.y
        );
        dst.femur.desired_relative_direction_standing =
            Quaternion.Inverse(src.femur.desired_relative_direction_standing);
        dst.tibia.desired_relative_direction_standing =
            Quaternion.Inverse(src.tibia.desired_relative_direction_standing);
        dst.femur_folding_direction = src.femur_folding_direction * -1;

        dst.femur.spriteRenderer.sprite = src.femur.spriteRenderer.sprite;
        dst.tibia.spriteRenderer.sprite = src.tibia.spriteRenderer.sprite;
        dst.femur.spriteRenderer.flipY = !src.femur.spriteRenderer.flipY;
        dst.tibia.spriteRenderer.flipY = !src.tibia.spriteRenderer.flipY;

        Span mirror_span(Span span) {
            return new Span(
                -span.max,
                -span.min
            );
        }

        copy_not_mirrored_leg_parameters(dst, src);
    }

    private static void copy_not_mirrored_leg_parameters(Leg dst, Leg src) {
        dst.tibia.attachment_point = src.femur.tip;
        dst.femur.rotation_speed = src.femur.rotation_speed;
        dst.tibia.rotation_speed = src.tibia.rotation_speed;
    }


}

}
