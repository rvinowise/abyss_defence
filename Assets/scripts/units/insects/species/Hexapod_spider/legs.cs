using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using geometry2d;
using rvinowise.units.parts.limbs;
using rvinowise.units.parts.limbs.init;
using rvinowise.units.parts.limbs.creeping_legs;
using rvinowise.units.parts;

namespace rvinowise.units.hexapod_spider.init {


static class Legs {
    private static readonly float scale = 1f * Settings.scale;
    private static Sprite sprite_femur;// = Resources.Load<Sprite>("sprites/basic_spider/femur.png");
    private static Sprite sprite_tibia;// = Resources.Load<Sprite>("sprites/basic_spider/tibia.png");*/

    public static void init(Creeping_leg_group @group) {
        sprite_femur = Resources.Load<Sprite>("basic_spider/femur");
        sprite_tibia = Resources.Load<Sprite>("basic_spider/tibia");
        
        List<Leg> legs = create_legs(@group);
        
        init_common_parameters(@group);
        init_parameters_that_shoud_be_mirrored(@group);

        init_parameters_that_can_be_inferred(@group);
        
        create_moving_strategy(@group);
    }

    private static void create_moving_strategy(Creeping_leg_group @group) {
        group.stable_leg_groups = new List<Stable_leg_group>() {
            new Stable_leg_group(
                new List<Leg>() {
                    @group.left_front_leg,
                    @group.right_hind_leg
                }
            ),
            new Stable_leg_group(
                new List<Leg>() {
                    @group.legs[4],
                    @group.legs[5]
                }
            ),
            new Stable_leg_group(
                new List<Leg>() {
                    @group.right_front_leg,
                    @group.left_hind_leg
                }
            )
        };

        @group.moving_strategy = new parts.limbs.creeping_legs.strategy.Stable(@group.legs, @group);
    }

    private static void init_parameters_that_shoud_be_mirrored(Creeping_leg_group @group) {
        init_left_front_leg(
            @group.left_front_leg
        );
        mirror(@group.right_front_leg, @group.left_front_leg);

        init_left_hind_leg(
            @group.left_hind_leg
        );
        mirror(@group.right_hind_leg, @group.left_hind_leg);

        init_left_middle_leg(
            @group.legs[4]);
        mirror(@group.legs[5], @group.legs[4]);
    }

    private static List<Leg> create_legs(Creeping_leg_group @group) {
        for (int i = 0; i < 6; i++) {
            @group.add_child(new Leg(@group.game_object.transform));
        }
        @group.left_front_leg.debug.name = "left_front_leg";
        @group.right_front_leg.debug.name = "right_front_leg";
        @group.left_hind_leg.debug.name = "left_hind_leg";
        @group.right_hind_leg.debug.name = "right_hind_leg";

        return @group.legs;
    }

    private const float rotation_speed = 360f;

    private static void init_left_front_leg(Leg leg) {
        leg.local_position = new Vector2(0.21f, 0.27f)* scale;
        leg.femur.possible_span = new Span(0f, 170f);
        //leg.femur.comfortable_span = leg.femur.possible_span.scaled(0.5f);
        leg.tibia.possible_span = new Span(-170f, 0f);
        //leg.tibia.comfortable_span = leg.tibia.possible_span.scaled(0.5f);

        leg.femur.desired_relative_direction_standing = Directions.degrees_to_quaternion(45f);
        leg.tibia.desired_relative_direction_standing = Directions.degrees_to_quaternion(-70f);
    }
    
    private static void init_left_middle_leg(Leg leg) {
        leg.local_position = new Vector2(0f, 0.43f) * scale;
        leg.femur.possible_span = new Span(20f, 160f);
        //leg.femur.comfortable_span = leg.femur.possible_span.scaled(0.5f);
        leg.tibia.possible_span = new Span(0f, 170f);
        //leg.tibia.comfortable_span = leg.tibia.possible_span.scaled(0.5f);
        leg.femur.desired_relative_direction_standing = Directions.degrees_to_quaternion(80f);
        leg.tibia.desired_relative_direction_standing = Directions.degrees_to_quaternion(40f);

    }

    private static void init_left_hind_leg(Leg leg) {
        leg.local_position = new Vector2(-0.31f, 0.33f) * scale;
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

    private static void init_parameters_that_can_be_inferred(Creeping_leg_group @group) {
        foreach (Leg leg in @group.legs) {
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

    
    private static void init_common_parameters(Creeping_leg_group @group) {
        foreach (Leg leg in @group.legs) {
            init_common_characteristic(leg);
        }
        @group.moving_offset_distance = 0.40f * scale;
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
