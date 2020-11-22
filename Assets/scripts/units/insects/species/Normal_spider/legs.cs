using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rvinowise.unity.extensions;

using rvinowise.unity.geometry2d;
using rvinowise.unity.units.parts.limbs.creeping_legs;
using rvinowise.unity.units.parts.limbs.creeping_legs.strategy;


namespace rvinowise.unity.units.normal_spider.init {
using parts.limbs;

static class Legs {
    private static readonly float scale = 1f * Settings.scale;

    public static void init(Creeping_leg_group @group) {
        
        List<Leg> legs = create_legs(@group);

        init_parameters_that_shoud_be_mirrored(@group);

        foreach (Leg leg in legs) {
            init_parameters_that_can_be_inferred(leg);
            init_common_parameters(leg);
        }

        create_moving_strategy(@group);
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
    }

    private static List<Leg> create_legs(Creeping_leg_group leg_group) {
        leg_group.right_front_leg = GameObject.Instantiate(leg_group.left_front_leg).GetComponent<Leg>();
        leg_group.right_hind_leg = GameObject.Instantiate(leg_group.left_hind_leg).GetComponent<Leg>();
        leg_group.right_front_leg.transform.parent = leg_group.transform;
        leg_group.right_hind_leg.transform.parent = leg_group.transform;
        
        leg_group.left_front_leg.debug.name = "left_front_leg";
        leg_group.right_front_leg.debug.name = "right_front_leg";
        leg_group.left_hind_leg.debug.name = "left_hind_leg";
        leg_group.right_hind_leg.debug.name = "right_hind_leg";

        return leg_group.legs;
    }

    

    private static void init_left_front_leg(Leg leg) {
        //leg.local_position = new Vector2(0.195f, 0.2925f) * scale;
        //leg.femur.tip = new Vector2(0.4225f, 0f) * scale;
        leg.femur.tip = leg.tibia.transform.localPosition;
        
        leg.tibia.tip = new Vector2(0.5525f, 0f) * scale;
        //leg.tibia.local_position = leg.femur.tip;

        leg.femur.desired_relative_direction_standing = Directions.degrees_to_quaternion(100f);
        leg.tibia.desired_relative_direction_standing = Directions.degrees_to_quaternion(-100f);
    }

    private static void init_left_hind_leg(Leg leg) {
        //leg.femur.tip = new Vector2(0.4225f, 0f) * scale;
        leg.femur.tip = leg.tibia.transform.localPosition;
        leg.tibia.tip = new Vector2(0.5525f, 0f) * scale;
        leg.femur.desired_relative_direction_standing = Directions.degrees_to_quaternion(100f);
        leg.tibia.desired_relative_direction_standing = Directions.degrees_to_quaternion(100f);
    }

    private static void init_parameters_that_can_be_inferred(Leg leg) {
        init_optimal_relative_position(leg);
        init_femur_folding_direction(leg);
    }

    
    private static void init_common_parameters(Leg leg) {
        leg.comfortable_distance = 0.5f * scale;
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

        //copy_not_mirrored_leg_parameters(dst, src);
    }

    private static void copy_not_mirrored_leg_parameters(Leg dst, Leg src) {
        dst.tibia.local_position = src.femur.tip;
        dst.femur.rotation_speed = src.femur.rotation_speed;
        dst.tibia.rotation_speed = src.tibia.rotation_speed;
        dst.transform.parent = src.transform.parent;
    }

    
    private static void create_moving_strategy(Creeping_leg_group @group) {
        @group.moving_strategy = new parts.limbs.creeping_legs.strategy.Stable(@group.legs, @group);
        group.stable_leg_groups = new List<Stable_leg_group>() {
            new Stable_leg_group(
                new List<Leg>() {@group.left_front_leg, @group.right_hind_leg}
            ),
            new Stable_leg_group(
                new List<Leg>() {@group.right_front_leg, @group.left_hind_leg}
            )
        };

    }

}

}
