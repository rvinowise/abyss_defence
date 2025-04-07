using rvinowise.unity.actions;
using rvinowise.unity.geometry2d;


namespace rvinowise.unity {

public static class Arm_pair_reloading {

    public static bool is_arm_reloading(Arm arm) {
        return arm.actor.current_action.is_part_of_action(typeof(Reload_pistol_simple));
    }
    
    public static Arm get_only_not_reloading_arm(Arm left_arm, Arm right_arm) {

        if (is_arm_reloading(left_arm) == is_arm_reloading(right_arm)) {
            return null;
        }
        if (is_arm_reloading(left_arm)) return right_arm;
        return left_arm;
    }

    
    public static bool is_reloadable_fully_loaded(IReloadable reloadable) {
        if (reloadable == null) return false;
        return reloadable.get_lacking_ammo() == 0;
    }
    
    public static bool both_arms_fully_loaded(Arm left_arm, Arm right_arm) {
        return
            is_reloadable_fully_loaded(left_arm.get_held_reloadable())
            &&
            is_reloadable_fully_loaded(right_arm.get_held_reloadable());
    }
    public static bool both_arms_are_reloading(Arm left_arm, Arm right_arm) {
        return
            is_arm_reloading(left_arm)
            &&
            is_arm_reloading(right_arm);
    }
    

    public static Arm get_the_only_arm_with_gun(Arm left_arm, Arm right_arm) {
        bool is_left = left_arm.get_held_gun() != null;
        bool is_right = right_arm.get_held_gun() != null;
        if (is_left == is_right) {
            return null;
        }
        if (is_left) return left_arm;
        return right_arm;
    }

    public static bool is_no_gun_present(Arm left_arm, Arm right_arm) {
        bool is_left = left_arm.get_held_gun() != null;
        bool is_right = right_arm.get_held_gun() != null;
        return !is_left && !is_right;
    }
    
    public static Arm get_best_arm_for_reloading(Arm_pair arm_pair) {
        Arm left_arm = arm_pair.left_arm;
        Arm right_arm = arm_pair.right_arm;
        
        if (both_arms_fully_loaded(left_arm, right_arm)) return null;
        if (is_no_gun_present(left_arm, right_arm)) return null;
        if (both_arms_are_reloading(left_arm, right_arm)) return null;

        Arm best_arm = null;
        
        var only_not_reloading_arm = get_only_not_reloading_arm(left_arm, right_arm);
        if (only_not_reloading_arm) {
            if (is_reloadable_fully_loaded(only_not_reloading_arm.get_held_reloadable())) {
                return null;
            }
            best_arm = only_not_reloading_arm;
        }

        if (get_the_only_arm_with_gun(left_arm, right_arm) is { } arm) {
            best_arm = arm;
        }
        
        if (best_arm!= null) {
            if (is_reloadable_fully_loaded(best_arm.get_held_reloadable())) {
                return null;
            }
            return best_arm;
        }
        
        var less_loaded_side = Reload_all.get_side_with_less_ammo(left_arm.held_tool, right_arm.held_tool);
        if (less_loaded_side != Side_type.NONE) {
            return Arm_pair_helpers.get_arm_on_side(arm_pair, less_loaded_side);
        }
        return right_arm;
    }
    
    public static void reload(Arm_pair arm_pair) {
        if (get_best_arm_for_reloading(arm_pair) is Arm reloaded_arm) {
            Action_sequential_parent.create(
                Reload_pistol_simple.create(
                    arm_pair, reloaded_arm, reloaded_arm.get_held_reloadable(), arm_pair.baggage
                )
            ).start_as_root(arm_pair.intelligence.action_runner);
        }
    }
    
}

}