using rvinowise.debug;
using rvinowise.unity;


namespace rvinowise.unity.actions {

public class Push_object_into_slot: Arm_reach_orientation {
    public Tool insertee;
    public Slot slot;
    private float old_rotation_acceleration;

    
    public static Push_object_into_slot create(
        Arm in_arm,
        Tool in_insertee,
        Slot in_slot
    ) {
        var action = (Push_object_into_slot)pool.get(typeof(Push_object_into_slot));
        action.arm = in_arm;
        action.insertee = in_insertee;
        action.slot = in_slot;
        return action;
    }


    protected override void on_start_execution() {
        base.on_start_execution();
        slow_movements(arm);
        desired_orientation = slot.get_orientation_inside();
    }

    protected override void restore_state() {
        restore_movements(arm);
    }
    
    private void slow_movements(Arm arm) {
        old_rotation_acceleration = arm.upper_arm.rotation_acceleration;
        arm.upper_arm.rotation_acceleration /= 10f;
        arm.upper_arm.current_rotation_inertia = 0;
        UnityEngine.Debug.Log($"Push_object_into_slot{id:N}: slowing {arm.name}, speed = {arm.upper_arm.rotation_acceleration}");
    }
    private void restore_movements(Arm arm) {
        arm.upper_arm.rotation_acceleration = old_rotation_acceleration;
        UnityEngine.Debug.Log($"Push_object_into_slot{id:N}{id:N}: restoring {arm.name}, speed = {arm.upper_arm.rotation_acceleration}");
    }
    
    public override void update() {
        desired_orientation.adjust_to_parent();
        base.update();
    }
}
}