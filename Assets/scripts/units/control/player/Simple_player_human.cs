using rvinowise.unity.actions;
using UnityEngine;


namespace rvinowise.unity {
public class Simple_player_human: Player_human {

    
    
    protected override void maybe_switch_items() {
        if (!switching_items_is_possible()) {
            return;
        }
        int wheel_steps = Player_input.instance.mouse_wheel_steps;
        if (wheel_steps == 0) {
            return;
        }
        int desired_tool_set = determine_current_selected_set(wheel_steps);
        if (desired_tool_set != toolset_equipper.current_equipped_set) {
            Debug.Log($"Mouse wheel is triggered, {wheel_steps} steps");
            toolset_equipper.equip_tool_set(desired_tool_set);
        }
    }

    private int determine_current_selected_set(int wheel_steps) {
        int desired_current_equipped_set = toolset_equipper.current_equipped_set + wheel_steps;
        desired_current_equipped_set = desired_current_equipped_set % baggage.tool_sets.Count;
        if (desired_current_equipped_set < 0) {
            desired_current_equipped_set = baggage.tool_sets.Count + desired_current_equipped_set;
        }
        return desired_current_equipped_set;
    }


    private bool was_attacking = false;
    
    protected override void use_tools() {
        bool wants_to_attack = Player_input.instance.button_presed("attack");

        if (wants_to_attack) {
            start_attack();
        } else if (was_attacking) {
            stop_attacking();
        }

        bool wants_to_reload = Player_input.instance.button_presed("reload");
        bool wants_to_power_tool = Player_input.instance.button_presed("power_tool");
        
        if (wants_to_reload) {
            Reload_all.create(user, this).start_as_root(action_runner);
        } else  if (wants_to_power_tool) {
            if (baggage.retrieve_current_powertool() is {} powertool) {
                //Attack_by_throwing_tool.create(user,powertool).start_as_root(action_runner);
            }
        }
        
        was_attacking = wants_to_attack;

    }
    
    private void start_attack() {
        if (get_selected_target() is {} target) {
            Debug.Log("AIMING: Simple_player_human.attack");
            arm_pair.attack(target);
        } else {
            Debug.Log("AIMING: attack (no target)");
            arm_pair.attack();
        }
        
    }

    private void stop_attacking() {
        arm_pair.stop_attacking();
    }
    
    
}
}