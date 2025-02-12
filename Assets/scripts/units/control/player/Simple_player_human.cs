using rvinowise.unity.actions;
using UnityEngine;


namespace rvinowise.unity {
public class Simple_player_human: Player_human {

    
    
    protected override bool maybe_switch_items() {
        if (!switching_items_is_possible()) {
            return false;
        }
        int wheel_steps = Player_input.instance.mouse_wheel_steps;
        if (wheel_steps == 0) {
            return false;
        }
        //toolset_equipper.switch_toolset_to_steps(wheel_steps);
        supertool_user.switch_supertool_to_steps(wheel_steps);
        return true;
    }


    private bool was_attacking = false;
    
    protected override bool use_tools() {
        bool wants_to_attack = //Player_input.instance.button_presed("attack");
            Input.GetButton("attack");
        bool wants_to_reload = Player_input.instance.is_button_presed("reload");
        bool wants_to_use_supertool = Player_input.instance.is_button_presed("supertool");
        
        
        if (wants_to_attack) {
            start_attack();
            Debug.Log("Simple_player_human wants_to_attack");
        } else if (was_attacking) {
            stop_attacking();
        }

        if (wants_to_reload) {
            Reload_all.create(user, this).start_as_root(action_runner);
        } else if (wants_to_use_supertool) {
            //arm_pair.use_supertool();
            // if (baggage.retrieve_current_powertool() is {} powertool) {
            //     Attack_by_throwing_tool.create(user,powertool).start_as_root(action_runner);
            // }
            supertool_user.use_desired_supertool();
        }
        
        was_attacking = wants_to_attack;
        
        return wants_to_attack || wants_to_reload || wants_to_use_supertool;
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