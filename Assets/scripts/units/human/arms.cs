using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rvinowise;
using rvinowise.units.equipment.limbs;
using units.equipment.arms;


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

        foreach (Arm arm in arms) {
            init_parameters_that_can_be_inferred(arm);
            init_common_parameters(arm);
        }

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

    private static void init_common_characteristic(Arm arm) {
        
    }

    private static void init_parameters_that_shoud_be_mirrored(Arm_controller controller) {
        init_left_arm(
            controller.arms[0],
            sprite_upper_arm,
            sprite_forearm
        );
        mirror(controller.arms[0], controller.arms[1]);
    }

    private static void init_left_arm(Arm arm, Sprite sprite_upper_arm, Sprite sprite_forearm) {
        
    }

    private static void mirror(Arm arm_src, Arm arm_dst) {
    }
}
}