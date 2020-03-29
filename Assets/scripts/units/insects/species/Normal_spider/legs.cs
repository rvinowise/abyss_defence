using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using geometry2d;
using rvinowise.units.parts.limbs.legs;
using rvinowise.units.parts.limbs.legs.strategy;


namespace rvinowise.units.normal_spider.init {
using parts.limbs;

static class Legs {
    private static readonly float scale = 1f * Settings.scale;
    private static Sprite sprite_femur;// = Resources.Load<Sprite>("sprites/basic_spider/femur.png");
    private static Sprite sprite_tibia;// = Resources.Load<Sprite>("sprites/basic_spider/tibia.png");

    public static Leg_controller init(Leg_controller controller) {
        sprite_femur = Resources.Load<Sprite>("basic_spider/femur");
        sprite_tibia = Resources.Load<Sprite>("basic_spider/tibia");
        
        List<Leg> legs = create_legs(controller);

        init_parameters_that_shoud_be_mirrored(controller);

        foreach (Leg leg in legs) {
            init_parameters_that_can_be_inferred(leg);
            init_common_parameters(leg);
        }

        create_moving_strategy(controller);
        return controller;
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
    }

    private static List<Leg> create_legs(Leg_controller controller) {
        for (int i = 0; i < 4; i++) {
            controller.add_tool(new Leg(controller.game_object.transform));
        }
        controller.left_front_leg.debug.name = "left_front_leg";
        controller.right_front_leg.debug.name = "right_front_leg";
        controller.left_hind_leg.debug.name = "left_hind_leg";
        controller.right_hind_leg.debug.name = "right_hind_leg";

        return controller.legs;
    }

    private const float rotation_speed = 360f;

    private static void init_left_front_leg(Leg leg, Sprite sprite_femur, Sprite sprite_tibia) {
        leg.local_position = new Vector2(0.195f, 0.2925f) * scale;
        leg.femur.possible_span = new Span(0f, 170f);
        //leg.femur.comfortable_span = leg.femur.possible_span.scaled(0.5f);
        leg.femur.tip = new Vector2(0.4225f, 0f) * scale;
        leg.femur.spriteRenderer.sprite = sprite_femur;
        leg.femur.rotation_speed = rotation_speed;
        leg.tibia.possible_span = new Span(-170f, 0f);
        //leg.tibia.comfortable_span = leg.tibia.possible_span.scaled(0.5f);
        leg.tibia.tip = new Vector2(0.5525f, 0f) * scale;
        leg.tibia.spriteRenderer.sprite = sprite_tibia;
        leg.tibia.local_position = leg.femur.tip;
        leg.tibia.rotation_speed = rotation_speed;

        leg.femur.desired_relative_direction_standing = Directions.degrees_to_quaternion(100f);
        leg.tibia.desired_relative_direction_standing = Directions.degrees_to_quaternion(-100f);
    }

    private static void init_left_hind_leg(Leg leg, Sprite sprite_femur, Sprite sprite_tibia) {
        leg.local_position = new Vector2(-0.2275f, 0.2925f) * scale;
        leg.femur.possible_span = new Span(10f, 180f);
        //leg.femur.comfortable_span = leg.femur.possible_span.scaled(0.5f);
        leg.femur.tip = new Vector2(0.4225f, 0f) * scale;
        leg.femur.spriteRenderer.sprite = sprite_femur;
        leg.femur.rotation_speed = rotation_speed;
        leg.tibia.possible_span = new Span(0f, 180f);
        //leg.tibia.comfortable_span = leg.tibia.possible_span.scaled(0.5f);
        leg.tibia.tip = new Vector2(0.5525f, 0f) * scale;
        leg.tibia.spriteRenderer.sprite = sprite_tibia;
        leg.tibia.local_position = leg.femur.tip;
        leg.tibia.rotation_speed = rotation_speed;
        leg.femur.desired_relative_direction_standing = Directions.degrees_to_quaternion(100f);
        leg.tibia.desired_relative_direction_standing = Directions.degrees_to_quaternion(100f);

    }

    private static void init_parameters_that_can_be_inferred(Leg leg) {
        init_optimal_relative_position(leg);
        init_femur_folding_direction(leg);
    }

    private static void init_common_parameters(Leg leg) {
        leg.comfortable_distance = 0.3f * scale;
    }

    private static void init_optimal_relative_position(Leg leg) {
        leg.optimal_relative_position_standing =
            leg.get_end_position_from_angles(
                leg.femur.desired_relative_direction_standing,
                leg.tibia.desired_relative_direction_standing
            );
    }

    private static void init_femur_folding_direction(Leg leg) {
        leg.folding_direction = -leg.tibia.possible_span.side_of_bigger_rotation();
    }

    private static void mirror(Leg dst, Leg src) {
        // the base direction_quaternion is to the right
        dst.local_position = new Vector2(
            src.local_position.x,
            -src.local_position.y
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
        //dst.folding_direction = -src.folding_direction; //inferred at the end

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
        dst.tibia.local_position = src.femur.tip;
        dst.femur.rotation_speed = src.femur.rotation_speed;
        dst.tibia.rotation_speed = src.tibia.rotation_speed;
    }

    
    private static void create_moving_strategy(Leg_controller controller) {
        controller.moving_strategy = new parts.limbs.legs.strategy.Stable(controller.legs) {
            stable_leg_groups = new List<Stable_leg_group>() {
                new Stable_leg_group(
                    new List<Leg>() {controller.left_front_leg, controller.right_hind_leg}
                ),
                new Stable_leg_group(
                    new List<Leg>() {controller.right_front_leg, controller.left_hind_leg}
                )
            }
        };
    }

}

}
