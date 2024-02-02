using UnityEngine;
using rvinowise.unity.units.parts.actions;
using rvinowise.unity.units.parts.limbs.arms;
using Action = rvinowise.unity.units.parts.actions.Action;


namespace units.human.actions {

public class Play_body_animation : Action_sequential_parent {


    private Arm left_arm;
    private Arm right_arm;
    private Animator animator;

    public static Play_body_animation create(
        Arm in_left_arm,
        Arm in_right_arm,
        Animator in_animator
    ) {
        Play_body_animation action = (Play_body_animation) pool.get(typeof(Play_body_animation));
        action.left_arm = in_left_arm;
        action.right_arm = in_right_arm;
        action.animator = in_animator;

        action.init_child_actions();
        return action;
    }

    private void init_child_actions() {

        Action_parallel_parent preparing_arms = Action_parallel_parent.create();
        //preparing_arms.add_child();
            
        add_child(preparing_arms);
        
        add_child(
            Play_recorded_animation.create(animator,0)
        );
    }

    public override void on_child_completed(Action in_sender_child) {
        base.on_child_completed(in_sender_child);
    }

    protected override void restore_state() {
        base.restore_state();
        adjust_desired_positions();
    }

    private void adjust_desired_positions() {
        left_arm.segment1.target_rotation = left_arm.segment1.rotation;
        left_arm.segment2.target_rotation = left_arm.segment2.rotation;
        
        right_arm.segment1.target_rotation = right_arm.segment1.rotation;
        right_arm.segment2.target_rotation = right_arm.segment2.rotation;
    }
}
}