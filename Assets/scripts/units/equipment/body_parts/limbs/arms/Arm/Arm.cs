﻿using rvinowise.unity.geometry2d;
using UnityEngine;

using rvinowise.contracts;
using rvinowise.unity.actions;
using System;
using rvinowise.unity.extensions;

namespace rvinowise.unity  {

public class Arm: 
    Limb2
    ,IReceive_recoil

{

    public Arm_pair pair;
    public Arm_segment shoulder;

    public Arm_segment upper_arm => segment1 as Arm_segment;
    public Arm_segment forearm => segment2 as Arm_segment;

    public Hand hand;

    public Tool held_tool => hand.held_tool;

    public Gun get_held_gun() {
        if (held_tool.GetComponent<Gun>() is {} gun) {
            return gun;
        }
        return null;
    }
    
    public Ammunition get_held_ammunition() {
        if (held_tool.GetComponent<Ammunition>() is {} ammunition) {
            return ammunition;
        }
        return null;
    }
    public Holding_place held_part => hand.held_part;

    public bool is_holding_tool() {
        return held_tool != null;
    }

    public float length {
        get { return upper_arm.length + forearm.length + hand.length; }
    }


    private Transform target;
    public Transform get_target() {
        if (current_action == null) {
            return null;
        }
        if (current_action.GetType() == typeof(Aim_at_target)) {
            return target;
        }
        // if ((target != null)&&(current_action.GetType() == typeof(Idle_vigilant_only_arm))) {
        //     Debug.LogError($"arm [{name}] has target [{target.name}], but the current action of the arm is [{current_action.get_explanation()}]");
        // }
        return null;
    }
    

    #region IReceive_recoil
    public void push_with_recoil(float in_impulse) {
        Side_type side = (Side_type) folding_side;
        shoulder.current_rotation_inertia += (float)side * in_impulse;
        upper_arm.current_rotation_inertia += (float)side * in_impulse;
        forearm.current_rotation_inertia += (float)Side.flipped(side) * in_impulse * 1.2f;
        hand.current_rotation_inertia += (float)side * in_impulse * 1.2f;
    }
    #endregion
    

    public Baggage baggage; 
    public Transform attention_target;

    

   
    public void start_idle_action() {
        Idle_vigilant_only_arm.create(
            this,
            attention_target,
            pair.transporter
        ).start_as_root(action_runner);
    }
    public void aim_at(Transform in_target) {
        Debug.Log($"AIMING: ({name})Arm.aim_at({in_target.name})");
        target = in_target;
        Aim_at_target.create(
            this,
            in_target,
            pair.transform
        ).start_as_root(action_runner);
    }

    private bool controlled_by_animation() {
        return pair.user.animator.isActiveAndEnabled;
    }
    
    public void rotate_to_orientation(Orientation needed_orientation) {
        set_desired_directions_by_position(needed_orientation.position);
        hand.set_target_rotation(needed_orientation.rotation);
        rotate_to_desired_directions();
    }
    public override void rotate_to_desired_directions() {
        shoulder.rotate_to_desired_direction();
        base.rotate_to_desired_directions();
        hand.rotate_to_desired_direction();
    }

    public override void jump_to_desired_directions() {
        base.jump_to_desired_directions();
        hand.jump_to_desired_direction();
    }

    public bool is_at_orientation(Orientation orientation) {
        const float touching_distance = 0.1f;
        if (
            (hand.position - orientation.position).magnitude <= touching_distance  &&
            hand.rotation.abs_degrees_to(orientation.rotation) <= Turning_element.rotation_epsilon
        ) 
        {
            return true;
        }
        return false;
    }
    
    public override bool at_desired_rotation() {
        return (
            base.at_desired_rotation() &&
            hand.at_desired_rotation()
        );
    }
    
    public void take_tool_from_baggage(Tool tool) {

        Action_sequential_parent.create(
            actions.Take_tool_from_bag.create(
                this, baggage, tool
            ),
            actions.Idle_vigilant_only_arm.create(
                this,
                attention_target,
                pair.transporter
            )
        ).start_as_root(action_runner);

    }

    public void support_held_tool(Tool tool) {
        Contract.Requires(hand.held_tool == null, "must be free in order to grab a tool");

        current_action = Action_sequential_parent.create(
            actions.Arm_reach_holding_part_of_tool.create(
                tool.second_holding
            ),
            actions.Attach_to_holding_part_of_tool.create(
                tool.second_holding
            )
        );
        
    }

    
    public float shoulder_mirrored_target_direction {
        set {
            set_relative_mirrored_target_direction(shoulder, value);
        }
    }

    public float get_aiming_distance_to(Transform in_target) {
        Quaternion needed_direction = transform.position.quaternion_to(in_target.position);
        return Math.Abs(hand.transform.rotation.degrees_to(needed_direction));
    }
    
    public bool aiming_automatically() {
        return 
        (get_held_gun() is {} gun)&&
        (gun.aiming_automatically);
    }

    public void draw_desired_directions(float time=0.1f) {
        rvinowise.unity.debug.Debug.DrawLine_simple(
            shoulder.position, 
            shoulder.desired_tip,
            Color.white,
            2
        );
        rvinowise.unity.debug.Debug.DrawLine_simple(
            shoulder.desired_tip, 
            upper_arm.desired_tip,
            Color.white,
            2
        );
        rvinowise.unity.debug.Debug.DrawLine_simple(
            upper_arm.desired_tip, 
            forearm.desired_tip,
            Color.white,
            2
        );
        rvinowise.unity.debug.Debug.DrawLine_simple(
            forearm.desired_tip,
            hand.desired_tip,
            Color.white,
            2
        );
    }

    public void drop_tool() {
        hand.detach_tool();
    }

    public void grab_tool(Holding_place in_place) {
        hand.attach_holding_part(in_place);
    }

#if UNITY_EDITOR
    protected override void OnDrawGizmos() {
        base.OnDrawGizmos();
        if (Application.isPlaying) {
            draw_desired_directions();
        }
    }
#endif
    
    public override void on_lacking_action() {
        Idle_vigilant_only_arm.create(
            this,
            Player_input.instance.cursor.transform,
            pair.transporter
        ).start_as_root(action_runner);
    }

}
}