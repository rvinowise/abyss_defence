using rvinowise.unity.geometry2d;
using UnityEngine;
using rvinowise.unity.extensions;
using rvinowise.unity;


namespace rvinowise.unity.actions {

public class Put_hand_before_bag: Action_of_arm {

    private Baggage bag;

    public static Put_hand_before_bag create(
        Arm in_arm,
        Baggage in_bag
    ) {
        var action = (Put_hand_before_bag)object_pool.get(typeof(Put_hand_before_bag));
        action.add_actor(in_arm);
        
        action.arm = in_arm;
        action.bag = in_bag;
        return action;
    }
    
    public Put_hand_before_bag() {
        
    }

    protected override void on_start_execution() {
        base.on_start_execution();

        if (arm.held_tool == null) {
            arm.hand.gesture = Hand_gesture.Open_sideview;
        }
    }
    public override void update() {
        base.update();
        
        arm.set_relative_mirrored_target_direction(arm.shoulder, 60f);
        Orientation desired_orientation = get_desired_orientation();
        arm.rotate_to_orientation(desired_orientation);
        if (is_reached_goal(desired_orientation)) {
            mark_as_completed();
        } else {
            mark_as_not_completed();
        }
    }
    
    private static Vector2 bag_offset = new Vector2(0.3f,0f);
    private Orientation get_desired_orientation() {
        return new Orientation(
            bag.position + (bag.rotation * bag_offset),
            bag.rotation * Directions.degrees_to_quaternion(180f),
            null
        );
    }

    private bool is_reached_goal(Orientation desired_orientation) {
        if (
            arm.hand.position.close_enough_to(desired_orientation.position) &&
            arm.hand.rotation.abs_degrees_to(desired_orientation.rotation) < bag.entering_span
        ) 
        {
            return true;
        }
        return false;
    }

}
}