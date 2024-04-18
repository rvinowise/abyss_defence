using rvinowise.debug;
using rvinowise.unity.geometry2d;
using rvinowise.unity.extensions;


namespace rvinowise.unity.actions {

public class Move_hand_into_bag: Action_of_arm {

    private Baggage bag;
    private float old_upper_arm_rotation_speed;
    private float old_shoulder_rotation_speed;

    
    public static Move_hand_into_bag create(
        Arm in_arm,
        Baggage in_bag
    ) {
        var action = (Move_hand_into_bag)object_pool.get(typeof(Move_hand_into_bag));
        action.add_actor(in_arm);
        
        action.arm = in_arm;
        action.bag = in_bag;
        return action;
    }
    public Move_hand_into_bag() {
        
    }


    protected override void on_start_execution() {
        base.on_start_execution();

        slow_movements(arm);
    }

    protected override void restore_state() {
        restore_movements(arm);
    }

    private void slow_movements(Arm arm) {
        old_upper_arm_rotation_speed = arm.upper_arm.rotation_acceleration;
        // arm.upper_arm.rotation_speed /= 2f;
        //
        old_shoulder_rotation_speed = arm.shoulder.rotation_acceleration;
        // arm.shoulder.rotation_speed /= 2f;

        arm.upper_arm.current_rotation_inertia = 0;
        arm.upper_arm.rotation_acceleration /= 10f;
        
        arm.shoulder.current_rotation_inertia = 0;
        arm.shoulder.rotation_acceleration /= 10f;
        if (arm.upper_arm.rotation_acceleration <=12f)
            UnityEngine.Debug.Log($"Move_hand_into_bag: over-slowing {arm.name}, speed = {arm.upper_arm.rotation_acceleration}");
        UnityEngine.Debug.Log($"Slowing speed by {get_explanation()}, speed = {arm.upper_arm.rotation_acceleration}");
    }
    private void restore_movements(Arm arm) {
        arm.upper_arm.rotation_acceleration = old_upper_arm_rotation_speed;
        arm.shoulder.rotation_acceleration = old_shoulder_rotation_speed;
        UnityEngine.Debug.Log($"Restoring speed by {get_explanation()}, speed = {arm.upper_arm.rotation_acceleration}");
    }


    public override void update() {
        base.update();
        
        arm.set_relative_mirrored_target_direction(arm.shoulder, 45f);
        Orientation desired_orientation = get_orientation_touching_baggage();
        arm.rotate_to_orientation(desired_orientation);
        if (complete(desired_orientation)) {
            mark_as_completed();
        } else {
            mark_as_not_completed();
        }
    }
    
    
    private Orientation get_orientation_touching_baggage() {
        return new Orientation(
            bag.position,// + (Vector2)(aggage.rotation * hand.tip),
            bag.rotation * Directions.degrees_to_quaternion(180f),
            null
        );
    }

    private bool complete(Orientation desired_orientation) {
        if (
            arm.hand.position.close_enough_to(desired_orientation.position) //&&
            //arm.hand.rotation.abs_degrees_to(desired_orientation.rotation) < bag.entering_span
            ) 
        {
            return true;
        }
        return false;
    }
}
}