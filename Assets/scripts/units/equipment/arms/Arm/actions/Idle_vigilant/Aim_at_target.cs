using System;
using System.Collections;
using System.Collections.Generic;
using rvinowise.unity.geometry2d;
using UnityEngine;
using rvinowise.unity.extensions;
using rvinowise.contracts;
using rvinowise.unity.units.parts.weapons.guns;

namespace rvinowise.unity.units.parts.limbs.arms.actions {

public class Aim_at_target: limbs.arms.actions.Action_of_arm {

    private Transform target;
    private Turning_element body; 
    
    public static Aim_at_target create(
        Arm in_arm,
        Transform in_target,
        Turning_element in_body
    ) {
        Contract.Assume(in_arm.held_tool is Gun, "aiming arm should hold a gun");

        var action = 
            (Aim_at_target)pool.get(typeof(Aim_at_target));
        action.actor = in_arm;
        
        action.arm = in_arm;
        action.set_target(in_target);
        action.body = in_body;
        return action;
    }
    
    public void set_target(Transform target) {
        this.target = target;
    }

    public Transform get_target() {
        return target;
    }
    
    public override void init_state() {
        base.init_state();
        arm.shoulder.set_target_direction_relative_to_parent(
            arm.shoulder.desired_idle_rotation
        );
        arm.shoulder.target_direction_relative = true;
        arm.upper_arm.target_direction_relative = false;
        arm.forearm.target_direction_relative = false;
        arm.hand.target_direction_relative = false;
        arm.shoulder.target_degree = new Degree(90f).adjust_to_side(arm.folding_side);
        arm.forearm.target_degree = 0f;
        arm.hand.target_degree = 0f;
    }
    
    
    public override void update() {
        
        Degree direction_to_target = arm.upper_arm.transform.degrees_to(target.position);
        //Degree final_body_direction = body.target_degree;
        Degree body_direction = body.transform.rotation.to_degree();
        Degree offset_from_body = body_direction.angle_to(direction_to_target);
        
        arm.shoulder.target_degree = get_shoulder_direction(
            offset_from_body,
            body_direction
        );

        arm.upper_arm.target_degree = get_upperarm_direction(
            offset_from_body,
            body_direction,
            direction_to_target
        );

        arm.forearm.target_degree = get_forearm_direction(
            offset_from_body,
            body_direction,
            direction_to_target
        );

        arm.hand.target_degree = direction_to_target;
        

        arm.rotate_to_desired_directions();
       
    }

    private Degree get_shoulder_direction(
        Degree offset_from_body,
        Degree final_body_direction
    ) {
        if (target_is_behind_my_straightened_shoulder()) {
            return offset_from_body;
        }

        Degree start_span = final_body_direction + arm.side.turn_degrees(90f);
        float span_width = 180f;
        Degree target_ratio = Math.Abs(
            new Degree(90).adjust_to_side(arm.side).angle_to(offset_from_body)
        ) / span_width;

        Degree max_sholder_shift = 45f;//arm.shoulder.possible_span.max;
        return new Degree(
                90f - (max_sholder_shift*target_ratio)
            ).adjust_to_side(arm.folding_side);

        #region local functions
        bool target_is_behind_my_straightened_shoulder() {
            return (
                (Mathf.Abs(offset_from_body) > 90f) &&
                (Math.Sign(offset_from_body) == arm.side)
            );
        }

        #endregion
    }
    
    private Degree get_upperarm_direction(
        Degree offset_from_body,
        Degree final_body_direction,
        Degree direction_to_target
    ) {
        if (target_is_on_my_side()) {
            return direction_to_target;
        }
        var desired_degree = direction_to_target + new Degree(
            Mathf.Abs(offset_from_body)*0.5f
        ).adjust_to_side(arm.side);

        return desired_degree;

        bool target_is_on_my_side() {
            return Side.from_degrees(offset_from_body) == arm.side;
        }
    }

    private Degree get_forearm_direction(
        Degree offset_from_body,
        Degree final_body_direction,
        Degree direction_to_target
    ) {
        if (target_is_on_my_side()) {
            return direction_to_target;
        }
        var desired_degree = direction_to_target + new Degree(
            Mathf.Abs(offset_from_body)*0.1f
        ).adjust_to_side(arm.side);

        return desired_degree;

        bool target_is_on_my_side() {
            return 
                (Mathf.Abs(offset_from_body) < 45f) ||
                (arm.side == Side.from_degrees(offset_from_body));
        }
    }

}
}