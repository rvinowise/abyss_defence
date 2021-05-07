using System;
using rvinowise.unity.geometry2d;
using rvinowise.unity.ui.input;
using rvinowise.unity.units.parts.limbs.arms;
using UnityEngine;

namespace rvinowise.unity.units.control.human {
public class Complex_player_human: Player_human {
    
    
    protected override void read_switching_items_input() {
        if (!switching_items_is_possible()) {
            return;
        }
        
        int wheel_steps = Player_input.instance.mouse_wheel_steps;

        if (Math.Abs(wheel_steps) > 0) {

            Arm selected_arm = get_selected_arm();
            /*selected_arm.take_tool_from_baggage(
                baggage.tool_sets[0]    
            );*/
        }
        return;
    }
    
    protected override void read_using_tools_input() {
        
        Side click_side = Side.NONE;
        if (UnityEngine.Input.GetMouseButtonDown(1)) {
            click_side = Side.RIGHT;
        } else if (UnityEngine.Input.GetMouseButtonDown(0)) {
            click_side = Side.LEFT;
        }
        if (click_side != Side.NONE) {
            attack(click_side);
        }

        Side rotation_side = get_selected_side();
    
        bool wants_to_reload = Player_input.instance.button_presed("reload");
        if (wants_to_reload) {
            arm_pair.wants_to_reload(rotation_side);
        }

    }


    private void attack(Side in_side) {
        bool wants_to_shoot_right = UnityEngine.Input.GetMouseButtonDown(1);
        bool wants_to_shoot_left = UnityEngine.Input.GetMouseButtonDown(0);
        if (one_button_for_both_arms()) {
            if (wants_to_shoot_left) {
                shoot();
            }
        } else {
            base.arm_pair.attack_with_arm(in_side);
            
        }
    }
    
    protected virtual void shoot() {
        if (get_selected_target() is Transform target) {
            arm_pair.attack(target);
        } else {
            arm_pair.attack();
        }
        
    }
    
    private bool one_button_for_both_arms() {
        bool no_matter_which_arm = 
            (arm_pair.left_arm.aiming_automatically())&&
            (arm_pair.right_arm.aiming_automatically());

        return no_matter_which_arm;
    }
    
}
}