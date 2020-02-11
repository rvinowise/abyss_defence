using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using geometry2d;
using System;


namespace units {
namespace limbs {
    
/* assume that the base direction is towards the right */
static class Legs_initializer {
    static public Vector2 scale = new Vector2(0.65f,0.65f);
    
    /// <summary>
    /// arranges the parameters of legs making them look like spider
    /// </summary>
    /// <param name="controller">the component whose legs are initialized by this</param>
    static public void init_for_spider(Leg_controller controller)
    {
        List<Leg> legs = create_legs(controller);

        init_parameters_that_shoud_be_mirrored(controller);

        foreach(Leg leg in legs) {
            init_parameters_that_can_be_inferred(leg);
            init_common_parameters(leg);
        }

        controller.stable_leg_groups = new List<Stable_leg_group>() {
            new Stable_leg_group(
                new List<Leg>() {
                    controller.left_front_leg,
                    controller.right_hind_leg
                }
            ),
            new Stable_leg_group(
                new List<Leg>() {
                    controller.right_front_leg,
                    controller.left_hind_leg
                }
            )
        };
    }

    private static void init_parameters_that_shoud_be_mirrored(Leg_controller controller)
    {
        init_left_front_leg(
            controller.left_front_leg,
            controller.sprite_femur,
            controller.sprite_tibia
        );
        mirror(controller.right_front_leg, controller.left_front_leg);

        init_left_hind_leg(
            controller.left_hind_leg,
            controller.sprite_femur,
            controller.sprite_tibia
        );
        mirror(controller.right_hind_leg, controller.left_hind_leg);
    }

    private static List<Leg> create_legs(Leg_controller controller)
    {
        controller.legs = new List<Leg>() {
            new Leg(controller.gameObject.transform),
            new Leg(controller.gameObject.transform),
            new Leg(controller.gameObject.transform),
            new Leg(controller.gameObject.transform)
        };
        return controller.legs;
    }

    /*private static void set_parents(Leg_controller controller) {
        Transform body = controller.gameObject.transform;
        foreach (Leg leg in controller.legs) {
            leg.femur.transform.SetParent(body);
            leg.tibia.transform.SetParent(leg.femur.transform);
        }
    }*/

    private static void init_left_front_leg(Leg leg, Sprite sprite_femur, Sprite sprite_tibia)
    {
        leg.attachment = new Vector2(0.30f, 0.45f) * scale;
        leg.femur.possible_span = new Span(0f, 170f);
        //leg.femur.comfortable_span = leg.femur.possible_span.scaled(0.5f);
        leg.femur.tip = new Vector2(0.65f, 0f) * scale;
        leg.femur.spriteRenderer.sprite = sprite_femur;
        leg.femur.rotation_speed = 6f;
        leg.tibia.possible_span = new Span(-170f, 0f);
        //leg.tibia.comfortable_span = leg.tibia.possible_span.scaled(0.5f);
        leg.tibia.tip = new Vector2(0.85f, 0f) * scale;
        leg.tibia.spriteRenderer.sprite = sprite_tibia;
        leg.tibia.attachment_point = leg.femur.tip;
        leg.tibia.rotation_speed = 6f;

        leg.femur.desired_relative_direction = Directions.degrees_to_quaternion(100f);
        leg.tibia.desired_relative_direction = Directions.degrees_to_quaternion(-100f);
    }
    
    private static void init_left_hind_leg(Leg leg, Sprite sprite_femur, Sprite sprite_tibia)
    {
        leg.attachment = new Vector2(-0.40f, 0.5f) * scale;
        leg.femur.possible_span = new Span(10f, 180f);
        //leg.femur.comfortable_span = leg.femur.possible_span.scaled(0.5f);
        leg.femur.tip = new Vector2(0.65f, 0f) * scale;
        leg.femur.spriteRenderer.sprite = sprite_femur;
        leg.femur.rotation_speed = 6f;
        leg.tibia.possible_span = new Span(0f, 180f);
        //leg.tibia.comfortable_span = leg.tibia.possible_span.scaled(0.5f);
        leg.tibia.tip = new Vector2(0.85f, 0f) * scale;
        leg.tibia.spriteRenderer.sprite = sprite_tibia;
        leg.tibia.attachment_point = leg.femur.tip;
        leg.tibia.rotation_speed = 6f;
        leg.femur.desired_relative_direction = Directions.degrees_to_quaternion(100f);
        leg.tibia.desired_relative_direction = Directions.degrees_to_quaternion(100f);
        
    }
    private static void init_parameters_that_can_be_inferred(Leg leg) {
        init_optimal_relative_position(leg);
        init_femur_folding_direction(leg);
    }
    private static void init_common_parameters(Leg leg) {
        leg.comfortable_distance = 0.3f;
    }

    private static void init_optimal_relative_position(Leg leg) {
        leg.optimal_relative_position = 
            leg.attachment + 
            (Vector2)(leg.femur.desired_relative_direction * leg.femur.tip) +
            (Vector2)(
                leg.tibia.desired_relative_direction *
                leg.femur.desired_relative_direction *
                leg.tibia.tip
            );
    }
    private static void init_femur_folding_direction(Leg leg) {
        leg.femur_folding_direction = -1*leg.tibia.possible_span.sign_of_bigger_rotation();
    }

    public static void mirror(Leg dst, Leg src) {
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
        dst.femur.desired_relative_direction =
            Quaternion.Inverse(src.femur.desired_relative_direction);
        dst.tibia.desired_relative_direction =
            Quaternion.Inverse(src.tibia.desired_relative_direction);
        dst.femur_folding_direction = src.femur_folding_direction *- 1;
        
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
}