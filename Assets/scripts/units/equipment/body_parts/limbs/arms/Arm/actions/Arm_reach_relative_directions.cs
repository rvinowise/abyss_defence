using rvinowise.unity.geometry2d;
using rvinowise.unity;

namespace rvinowise.unity.actions {

public class Arm_reach_relative_directions: Action_leaf {
    private Arm arm;

    Degree shoulder_rotation;
    Degree upper_arm_rotation;
    Degree forearm_rotation;
    Degree hand_rotation;
    
    public static Action create_assuming_left_arm(
        Arm in_arm, 
        Degree in_shoulder_rotation,
        Degree in_upper_arm_rotation,
        Degree in_forearm_rotation,
        Degree in_hand_rotation
    ) {
        
        var action = (Arm_reach_relative_directions)object_pool.get(typeof(Arm_reach_relative_directions));
        
        action.add_actor(in_arm);
        action.arm = in_arm;
        if (in_arm.side == Side_type.LEFT) {
            action.shoulder_rotation = in_shoulder_rotation;
            action.upper_arm_rotation = in_upper_arm_rotation;
            action.forearm_rotation = in_forearm_rotation;
            action.hand_rotation = in_hand_rotation;
        } else {
            action.shoulder_rotation = -in_shoulder_rotation;
            action.upper_arm_rotation = -in_upper_arm_rotation;
            action.forearm_rotation = -in_forearm_rotation;
            action.hand_rotation = -in_hand_rotation;
        } 
        
        return action;
    }

    protected override void on_start_execution() {
        base.on_start_execution();
        arm.shoulder.set_target_direction_relative_to_parent(
            shoulder_rotation
        );
        arm.upper_arm.set_target_direction_relative_to_parent(
            upper_arm_rotation
        );
        arm.forearm.set_target_direction_relative_to_parent(
            forearm_rotation
        );
        arm.hand.set_target_direction_relative_to_parent(
            hand_rotation
        );
    }

    protected override void restore_state() {
        base.restore_state();
        arm.shoulder.target_direction_relative = false;
        arm.upper_arm.target_direction_relative = false;
        arm.forearm.target_direction_relative = false;
        arm.hand.target_direction_relative = false;
    }

    public override void update() {
        if (complete()) {
            mark_as_completed();
        } else {
            arm.rotate_to_desired_directions();
        }
    }
    

    protected bool complete() {
        return arm.at_desired_rotation();
    }

}
}