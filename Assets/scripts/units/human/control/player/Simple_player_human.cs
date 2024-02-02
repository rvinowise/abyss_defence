using rvinowise.unity.ui.input;
using rvinowise.unity.units.parts.limbs.arms.actions;
using UnityEngine;


namespace rvinowise.unity.units.control.human {
public class Simple_player_human: Player_human {

    
    
    protected override void read_switching_items_input() {
        if (!switching_items_is_possible()) {
            return;
        }
        int wheel_steps = Player_input.instance.mouse_wheel_steps;
        if (wheel_steps == 0) {
            return;
        }
        int desired_tool_set = determine_current_selected_set(wheel_steps);
        if (desired_tool_set != current_equipped_set) {
            equip_tool_set(desired_tool_set);
        }
    }

    private int determine_current_selected_set(int wheel_steps) {
        int desired_current_equipped_set = current_equipped_set + wheel_steps;
        desired_current_equipped_set = desired_current_equipped_set % baggage.tool_sets.Count;
        if (desired_current_equipped_set < 0) {
            desired_current_equipped_set = baggage.tool_sets.Count + desired_current_equipped_set;
        }
        return desired_current_equipped_set;
    }

    private bool wants_to_shoot;
    protected override void Update()
    {
        base.Update();
        if (Input.GetMouseButtonDown(0)) {
            wants_to_shoot = true;
        }
    }
    
    private void equip_tool_set(int set_index) {
        current_equipped_set = set_index;
        Equip_toolset.create(
            this,
            baggage.tool_sets[set_index]
        ).add_marker("changing tool").start_as_root(action_runner);
    }
    
    protected override void read_using_tools_input() {
        bool wants_to_attack = UnityEngine.Input.GetMouseButton(0);

        if (wants_to_attack) {
            attack();
        }

        bool wants_to_reload = Player_input.instance.button_presed("reload");
        if (wants_to_reload) {
            Reload_all.create(user, this).start_as_root(action_runner);
        }

    }
    
    
    
    protected virtual void attack() {
        if (get_selected_target() is Transform target) {
            arm_pair.attack(target);
            //Debug.Log("attack!!!");
        } else {
            arm_pair.attack();
        }
        
    }
    
    
}
}