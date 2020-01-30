using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using geometry2d;

namespace units {
namespace limbs {
    
/* assume that the base direction is towards the right */
static class Legs_initializer {
    static public Vector2 scale = new Vector2(0.2f,0.2f);
    
    static public Leg_controller init_for_spider(Leg_controller controller)
    {
        //Leg_controller controller = game_object.AddComponent<Leg_controller>();
        controller.legs = new List<Leg>() {
            /*new Leg(controller.gameObject.transform),
            new Leg(controller.gameObject.transform),
            new Leg(controller.gameObject.transform),*/
            new Leg(controller.gameObject.transform)
        };

        init_left_front_leg(
            controller.left_front_leg,
            controller.sprite_femur,
            controller.sprite_tibia
        );
        /*init_left_front_leg(
            controller.right_front_leg,
            controller.sprite_femur,
            controller.sprite_tibia
        );
        controller.right_front_leg.mirror(controller.left_front_leg);

        init_left_hind_leg(
            controller.left_hind_leg,
            controller.sprite_femur,
            controller.sprite_tibia
        );
        controller.right_hind_leg.mirror(controller.left_hind_leg);
*/

        //set_parents(controller);

        return controller;
    }

    private static void set_parents(Leg_controller controller) {
        Transform body = controller.gameObject.transform;
        foreach (Leg leg in controller.legs) {
            leg.femur.transform.SetParent(body);
            leg.tibia.transform.SetParent(leg.femur.transform);
        }
    }

    private static void init_left_front_leg(Leg leg, Sprite sprite_femur, Sprite sprite_tibia)
    {
        leg.attachment = new Vector2(0.40f, 0.5f) * scale;
        leg.femur.span = new Span(0f, 170f);
        leg.femur.tip = new Vector2(0.65f, 0f) * scale;
        leg.femur.spriteRenderer.sprite = sprite_femur;
        leg.tibia.span = new Span(-170f, 0f);
        leg.tibia.tip = new Vector2(0.65f, 0f) * scale;
        leg.tibia.spriteRenderer.sprite = sprite_tibia;
        leg.optimal_aim = leg.deduce_optimal_aim();
    }

    private static void init_left_hind_leg(Leg leg, Sprite sprite_femur, Sprite sprite_tibia)
    {
        leg.attachment = new Vector2(-0.40f, 0.5f) * scale;
        leg.femur.span = new Span(10f, 180f);
        leg.femur.tip = new Vector2(0.65f, 0f) * scale;
        leg.femur.spriteRenderer.sprite = sprite_femur;
        leg.tibia.span = new Span(0f, 180f);
        leg.tibia.tip = new Vector2(0.65f, 0f) * scale;
        leg.tibia.spriteRenderer.sprite = sprite_tibia;
    }
}

}    
}