using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rvinowise.unity.extensions;

using rvinowise.unity.geometry2d;
using rvinowise.unity.units.parts.limbs.creeping_legs;
using rvinowise.unity.units.parts.limbs.creeping_legs.strategy;


namespace rvinowise.unity.units.normal_spider.init {
    using System.Linq;
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
        ensure_space_for_legs();

        leg_group.right_front_leg = copy_mirrored_leg(leg_group, leg_group.left_front_leg);
        leg_group.right_hind_leg = copy_mirrored_leg(leg_group, leg_group.left_hind_leg);
    
        leg_group.left_front_leg.name = "left_front_leg";
        leg_group.right_front_leg.name = "right_front_leg";
        leg_group.left_hind_leg.name = "left_hind_leg";
        leg_group.right_hind_leg.name = "right_hind_leg";

        return leg_group.legs;

        void ensure_space_for_legs() {
            int needed_legs_qty = 4;
            int initial_legs_qty = leg_group.legs.Count;
            foreach(int i in Enumerable.Range(0, needed_legs_qty-initial_legs_qty)) {
                leg_group.legs.Add(null);
            }
        }
        
    }

    private static Leg copy_mirrored_leg(
        Creeping_leg_group leg_group,
        Leg src_leg
    ) {
        Leg dst_leg = GameObject.Instantiate(src_leg).GetComponent<Leg>();
        //dst_leg.transform.localScale = leg_group.transform.localScale;
        dst_leg.transform.SetParent(leg_group.transform, false);

        return dst_leg;
    }

    

    private static void init_left_front_leg(Leg leg) {
        leg.femur.tip = leg.tibia.transform.localPosition;
        leg.tibia.tip = leg.tip.localPosition;
    }

    private static void init_left_hind_leg(Leg leg) {
        leg.femur.tip = leg.tibia.transform.localPosition;
        leg.tibia.tip = leg.tip.localPosition;
    }

    private static void init_parameters_that_can_be_inferred(Leg leg) {
        //init_optimal_relative_position(leg);
        init_femur_folding_direction(leg);
    }

    
    private static void init_common_parameters(Leg leg) {
        leg.comfortable_distance = 0.5f * leg.transform.lossyScale.x * scale;
    }

    private static void init_optimal_relative_position(Leg leg) {
        leg.optimal_relative_position_standing_transform.localPosition =
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
        dst.femur.tip = new Vector2(
            src.femur.tip.x,
            -src.femur.tip.y
        );

        dst.tibia.possible_span = mirror_span(src.tibia.possible_span);
        dst.tibia.tip = new Vector2(
            src.tibia.tip.x,
            -src.tibia.tip.y
        );
        dst.femur.desired_relative_direction_standing =
            Quaternion.Inverse(src.femur.desired_relative_direction_standing);
        dst.tibia.desired_relative_direction_standing =
            Quaternion.Inverse(src.tibia.desired_relative_direction_standing);

        dst.femur.sprite_renderer.flipY = !src.femur.sprite_renderer.flipY;
        dst.tibia.sprite_renderer.flipY = !src.tibia.sprite_renderer.flipY;

        dst.optimal_relative_position_standing_transform.localPosition =
            mirror_point(src.optimal_relative_position_standing_transform.localPosition);


        Span mirror_span(Span span) {
            return new Span(
                -span.max,
                -span.min
            );
        }

        Vector3 mirror_point(Vector3 in_point) {
            return new Vector3(
                in_point.x,
                -in_point.y,
                in_point.z
            );
        }

        //copy_not_mirrored_leg_parameters(dst, src);
    }

    private static void copy_not_mirrored_leg_parameters(Leg dst, Leg src) {
        dst.tibia.localPosition = src.femur.tip;
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
