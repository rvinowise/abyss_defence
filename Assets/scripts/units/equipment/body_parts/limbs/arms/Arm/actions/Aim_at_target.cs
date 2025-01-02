using System;
using rvinowise.unity.geometry2d;
using UnityEngine;
using rvinowise.unity.extensions;
using rvinowise.contracts;

namespace rvinowise.unity.actions {

public class Aim_at_target: Action_of_arm {

    protected Transform target;
    protected Transform body; 
    
    public static Aim_at_target create(
        Arm in_arm,
        Transform in_target,
        Transform in_body
    ) {
        Contract.Assume(in_arm.get_held_gun() != null, "aiming arm should hold a gun");

        var action = 
            (Aim_at_target)object_pool.get(typeof(Aim_at_target));
        action.add_actor(in_arm);
        
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

    protected override void on_start_execution() {
        if (target != null) {
            Debug.Log($"AIMING: ({this.arm.name})Aim_at_target.on_start_execution({this.target.name})");
        }
        else {
            Debug.Log($"AIMING: ({this.arm.name})Aim_at_target.on_start_execution(target=NULL)");    
        }
        
        base.on_start_execution();
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
        if (target == null) {
            mark_as_completed();
        }
        else { 
            Degree direction_from_upperarm_to_target = 
                arm.upper_arm.transform.degrees_to(target.position);
            Degree body_direction = body.rotation.to_degree();
            Degree offset_from_body = body_direction.angle_to(direction_from_upperarm_to_target);

            arm.shoulder.target_degree = get_shoulder_direction(
                offset_from_body,
                body_direction
            );

            arm.upper_arm.target_degree = get_upperarm_direction(
                offset_from_body,
                direction_from_upperarm_to_target
            );

            arm.forearm.target_degree = get_forearm_direction(
                offset_from_body,
                direction_from_upperarm_to_target
            );

            Degree direction_from_hand_to_target = 
                arm.hand.transform.degrees_to(target.position);
            arm.hand.target_degree = direction_from_hand_to_target;


            arm.rotate_to_desired_directions();
        }
    }

    private Degree get_shoulder_direction(
        Degree offset_from_body,
        Degree final_body_direction
    ) {
        if (target_is_behind_my_straightened_shoulder()) {
            return offset_from_body;
        }

        Degree start_span = final_body_direction + Side.turn_degrees(arm.side, 90f);
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
                (Math.Sign(offset_from_body) == (int)arm.side)
            );
        }

        #endregion
    }
    
    private Degree get_upperarm_direction(
        Degree offset_from_body,
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