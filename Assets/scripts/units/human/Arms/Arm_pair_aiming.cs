using System;
using System.Collections.Generic;
using System.Linq;
using rvinowise.unity.actions;
using rvinowise.unity.geometry2d;
using UnityEngine;


namespace rvinowise.unity {

public static class Arm_pair_aiming {

    
    
    public static void aim_at_hinted_targets(Arm_pair arm_pair) {
        
        var aiming_arms = get_arms_searching_for_targets(arm_pair);
        if (!aiming_arms.Any()) return;


        List<Transform> hinted_targets = new List<Transform>(arm_pair.team.get_enemy_transforms());
        hinted_targets.AddRange(arm_pair.team.get_enemy_targetables());
        
        if (arm_pair.get_explicit_target() is {} explicit_target) {
            hinted_targets = hinted_targets
                .Prepend(Player_input.instance.cursor.transform)
                .ToList();
        }
        
        var hinted_targets_sorted = Finding_objects.components_sorted_by_distance(
            Player_input.instance.cursor.transform.position,
            hinted_targets
        );
        // var hinted_targets =
        //     Finding_objects.components_sorted_by_distance(
        //         Player_input.instance.cursor.transform.position,
        //         arm_pair.team.get_enemy_transforms()
        //     );
        
        
        var needed_targets =
            hinted_targets_sorted.Take(aiming_arms.Count).Select(tuple => tuple.Item1).ToList();
        
        var current_targets =
            new HashSet<Transform>(get_all_targets(arm_pair));

        var needed_not_targeted_enemies =
            new HashSet<Transform>(needed_targets.Except(current_targets));

        var arms_changing_targets =
            aiming_arms
                .Where(arm => !needed_targets.Contains(arm.get_target()))
                .ToList();
        
        foreach (var target in needed_not_targeted_enemies) {
            arm_pair.get_arm_closest_to<IGun>(arms_changing_targets, target)
                ?.start_aiming_at_target(target);
        }
        
    }

    public static List<Arm> get_arms_searching_for_targets(Arm_pair arm_pair) {
        return arm_pair.arms.Where(is_arm_searching_for_targets).ToList();
    }
    public static bool is_arm_searching_for_targets(Arm in_arm) {
        return
            (is_arm_autoaimed(in_arm))
            &&
            (
                (in_arm.actor.current_action == null)
                ||
                (in_arm.actor.current_action.GetType() == typeof(Idle_vigilant_only_arm))
                ||
                (in_arm.actor.current_action.GetType() == typeof(Aim_at_target))
            )
            &&(in_arm.get_held_gun() is { } gun)&&(in_arm.get_held_reloadable()?.get_loaded_ammo() > 0);
    }



    public static List<Arm> get_all_armed_autoaimed_arms(Arm_pair arm_pair) {
        List<Arm> arms = new List<Arm>();

        if (is_arm_autoaimed(arm_pair.right_arm))
        {
            arms.Add(arm_pair.right_arm);
        }
        if (is_arm_autoaimed(arm_pair.left_arm))
        {
            arms.Add(arm_pair.left_arm);
        }
        return arms;
    }
    
    public static void set_target_for(Arm in_arm, Transform in_target) {
        in_arm.start_aiming_at_target(in_target);
    }

    public static List<Transform> get_all_targets(Arm_pair arm_pair) {
        List<Transform> result = new List<Transform>();
        foreach (Arm arm in get_all_armed_autoaimed_arms(arm_pair)) {
            if (arm.get_target() != null) {
                result.Add(arm.get_target());
            }
        }
        return result;
    }
    
    public static void try_find_new_target(Arm_pair arm_pair, Arm in_arm) {
        Debug.Log($"AIMING: try_find_new_target({in_arm.name})");
        List<Transform> free_enemies = get_not_targeted_enemies(arm_pair);
        Distance_to_component closest_target = Object_finder.instance.get_closest_object(
            Player_input.instance.mouse_world_position,
            free_enemies
        );
        if (closest_target.get_transform() != null) {
            set_target_for(in_arm, closest_target.get_transform());
        }
        else {
            in_arm.start_idle_action();
        }
    }
    private static List<Transform> get_not_targeted_enemies(Arm_pair arm_pair) {
        return arm_pair.team.get_enemy_transforms().Except(get_all_targets(arm_pair)).ToList();
    }

    public static bool is_arm_autoaimed(Arm in_arm) {
        if (in_arm.get_held_gun()?.is_aiming_automatically() == true) {
            return true;
        }
        return false;
    }

    private static bool is_aiming_at(Arm_pair arm_pair, Transform in_target) {
        return get_all_targets(arm_pair).IndexOf(in_target) > -1;
    }


    public static Transform get_target_of(Arm in_arm) {
        if (in_arm.actor.current_action is Aim_at_target aiming) {
            return aiming.get_target();
        }
        return null;
    }
    public static Arm get_arm_targeting(Arm_pair arm_pair, Transform in_target) {
        var left_arm = arm_pair.left_arm;
        var right_arm = arm_pair.right_arm;
        if (
            left_arm.actor.current_action is Aim_at_target left_aiming&&
            left_arm.get_target() == in_target
        ) {
            return left_arm;
        } else if (
            right_arm.actor.current_action is Aim_at_target right_aiming&&
            right_arm.get_target() == in_target 
        ) {
            return right_arm;
        }
        return null;
    }
    
}

}