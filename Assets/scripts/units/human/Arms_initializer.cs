using System;
using System.Collections;
using System.Collections.Generic;
using geometry2d;
using UnityEngine;
using rvinowise;
using rvinowise.units.parts;
using rvinowise.units.parts.limbs;
using rvinowise.units.parts.limbs.arms;
using Segment = rvinowise.units.parts.limbs.arms.Segment;

using Arm_controller = rvinowise.units.parts.limbs.arms.humanoid.Arm_controller;
using rvinowise.units.parts.limbs.arms.humanoid;


namespace rvinowise.units.humanoid.init {

public class Arms_initializer: MonoBehaviour
{
    [Header("left arm bones")]
    public Segment shoulder_l;
    public Segment upper_arm_l;
    public Segment forearm_l;
    public parts.limbs.arms.Hand hand_l;
    
    [Header("right arm bones")]
    public Segment shoulder_r;
    public Segment upper_arm_r;
    public Segment forearm_r;
    public parts.limbs.arms.Hand hand_r;

    private static readonly Vector2 scale = new Vector2(1f, 1f);

    private static Baggage baggage;
    private static Transform idle_target;
    
    public Arm_controller init(
        Arm_controller controller,
        Baggage in_baggage,
        Transform in_idle_target
    ) {
        baggage = in_baggage;
        idle_target = in_idle_target;
        
        IList<Arm> arms = create_arms(controller);

        init_common_parameters(controller);
        
        init_parameters_that_shoud_be_mirrored(controller);
        foreach (Arm arm in arms) {
            init_arm_parameters_that_can_be_inferred(arm);
        }
        init_parameters_that_can_be_inferred(controller);

        return controller;
    }

    

    private IList<Arm> create_arms(Arm_controller controller) {
        controller.add_child(
            new Arm(
                controller,
                //rvinowise.ui.input.Input.instance.cursor.center.transform,
                shoulder_l,
                upper_arm_l,
                forearm_l,
                hand_l
            )
        );
        controller.add_child(
            new Arm(
                controller,
                //rvinowise.ui.input.Input.instance.cursor.center.transform,
                shoulder_r,
                upper_arm_r,
                forearm_r,
                hand_r
            )
        );

        return controller.arms;
    }

    private static void init_common_parameters(Arm_controller controller) {
        foreach (Arm arm in controller.arms) {
            init_common_characteristic(arm, controller.transform);
        }
    }

    private const float rotation_speed = 240f;
    private static void init_common_characteristic(Arm arm, Transform parent) {
        
        /*arm.shoulder.rotation_speed = 10f;
        arm.upper_arm.rotation_speed = 200f;
        arm.forearm.rotation_speed = 400f;
        arm.hand.rotation_speed = 300f;*/
        
        arm.shoulder.rotation_acceleration = 400f;
        arm.upper_arm.rotation_acceleration = 600f;
        arm.forearm.rotation_acceleration = 600f;
        arm.hand.rotation_acceleration = 800f;

        arm.baggage = baggage;
        arm.idle_target = idle_target;
    }

    private static void init_parameters_that_shoud_be_mirrored(Arm_controller controller) {
        init_left_arm(
            controller.left_arm
        );
        mirror(controller.right_arm, controller.left_arm);
    }

    private static void init_left_arm(Arm arm) {
        
        arm.shoulder.possible_span = new Span(120f, 45f);
        //arm.shoulder.possible_span = new Span(110f, 80f);
        //arm.shoulder.length = (arm.shoulder.position - arm.upper_arm.position).magnitude;
        arm.shoulder.init_length_to(arm.upper_arm);
        arm.shoulder.desired_idle_direction = Directions.degrees_to_quaternion(90f);

        arm.upper_arm.possible_span = new Span(10f, -140f);
        //arm.upper_arm.length = (arm.upper_arm.position - arm.forearm.position).magnitude;
        arm.upper_arm.init_length_to(arm.forearm);
        arm.upper_arm.desired_idle_direction = Directions.degrees_to_quaternion(20f);
        
        arm.forearm.possible_span = new Span(0f, -150f);
        //arm.forearm.possible_span = new Span(0f, -170f);
        arm.forearm.init_length_to(arm.hand);
        arm.forearm.desired_idle_direction = Directions.degrees_to_quaternion(-20f);
        
        //arm.hand.possible_span = new Span(80f, -80f);
        arm.hand.possible_span = new Span(20f, -80f);
        arm.hand.desired_idle_direction = Directions.degrees_to_quaternion(0f);
    }

    private static void mirror(Arm arm_dst , Arm arm_src) {
        arm_dst.shoulder.mirror_from(arm_src.shoulder);
        parts.limbs.init.Initializer.mirror(arm_dst, arm_src);
    }

    private static void init_arm_parameters_that_can_be_inferred(Arm arm) {
        arm.folding_direction = -arm.segment2.possible_span.side_of_bigger_rotation();
    }
    private static void init_parameters_that_can_be_inferred(Arm_controller controller) {
        controller.shoulder_span =
            (controller.left_arm.local_position - controller.right_arm.local_position).
            magnitude;
    }
}
}