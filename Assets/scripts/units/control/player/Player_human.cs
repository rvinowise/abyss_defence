using System.Collections.Generic;
using UnityEngine;
using rvinowise.unity.extensions;

using static rvinowise.unity.geometry2d.Directions;
using rvinowise.contracts;
using System.Linq;
using rvinowise.unity.actions;
using rvinowise.unity.geometry2d;

namespace rvinowise.unity {

public abstract class Player_human : Human_intelligence {

    private int[] held_tool_index;
    private Transform cursor_transform;


    protected override void Start() {
        base.Start();
        cursor_transform = Player_input.instance.cursor.transform;
        consider_all_enemies();
        sensory_organ?.pay_attention_to_target(Player_input.instance.cursor.transform);
    }

    private void consider_all_enemies() {
        foreach(Team enemy_team in team.enemy_teams) {
            foreach(var enemy_unit in enemy_team.units) {
                on_enemy_appeared(enemy_unit);
            }
        }
    }

    protected void Update() {
        move_in_space();
        maybe_switch_items();
        use_tools();
        
        if (Input.GetMouseButton(0))
            Debug.Log("Pressed left-click.");
        
        action_runner.update();
    }

    protected abstract void use_tools();

    private Arm get_arm_targeting_selected_target() {
        Distance_to_component closest = Distance_to_component.empty();
        foreach(Arm arm in arm_pair.get_all_armed_autoaimed_arms()) {
            if (arm_pair.get_target_of(arm) is Transform target) {
                float this_distance = target.sqr_distance_to(Player_input.instance.cursor.transform.position);
                if (this_distance < closest.distance) {
                    closest = new Distance_to_component(arm, this_distance);
                }
            }
        }
        return closest.component as Arm;
    }

    protected Transform get_selected_target() {
        Distance_to_component closest = Distance_to_component.empty();
        foreach(Transform target in arm_pair.get_all_targets()) {
            float this_distance = target.sqr_distance_to(Player_input.instance.cursor.transform.position);
            if (this_distance < closest.distance) {
                closest = new Distance_to_component(target, this_distance);
            }
        }
        return closest.component as Transform;
    }
    

    protected abstract void maybe_switch_items();

    public Arm get_selected_arm() {
        return arm_pair.get_arm_on_side(get_selected_side());
    }
    
    public Side_type get_selected_side() {
        return Side.from_degrees(last_rotation);
    }


    protected bool switching_items_is_possible() {
        if (baggage == null) {
            return false;
        }
        if (Player_input.instance.zoom_held) {
            return false;
        }
        return true;
    }

    private void move_in_space() {
        if (transporter == null) {
            return;
        }
        var destination = (Vector2)transform.position+Player_input.instance.moving_vector;
        transporter.move_towards_destination(destination);
        
        Vector2 mouse_pos = Player_input.instance.cursor.transform.position;
        Quaternion needed_direction = (mouse_pos - (Vector2) transform.position).to_quaternion();
        transporter.face_rotation(needed_direction);
        
        Vector2 read_moving_direction() {
            Vector2 direction_vector = Player_input.instance.moving_vector;
            return direction_vector.normalized;
        }

        Quaternion read_face_direction() {
            Vector2 mousePos = Player_input.instance.cursor.transform.position;
            Quaternion needed_direction = (mousePos - (Vector2) transform.position).to_quaternion();
            if (has_gun_in_2hands(out var gun))
            {
                needed_direction *= get_additional_rotation_for_2hands_gun(gun); 
                    
            }
            save_last_rotation(needed_direction);

            return needed_direction;
        }
    }

    private bool has_gun_in_2hands(out Gun out_gun) {
        if (
            (arm_pair?.right_arm.current_action is Idle_vigilant_main_arm) &&
            (arm_pair?.right_arm.held_tool is Gun gun)
        ) {
            out_gun = gun;
            return true;
        }
        out_gun = null;
        return false;
    }

    private Quaternion get_additional_rotation_for_2hands_gun(Gun gun) {
        
        float body_rotation =
            Triangles.get_angle_by_lengths(
                arm_pair.shoulder_span,
                gun.butt_to_second_grip_distance,
                arm_pair.left_arm.length- arm_pair.left_arm.hand.length
            ) -90f;
        
        
        if (float.IsNaN(body_rotation)) {
            return Quaternion.identity;
        }
        
        return degrees_to_quaternion(body_rotation);
    }


    public override void on_enemy_appeared(Intelligence in_enemy) {
        if (arm_pair.get_iddling_armed_autoaimed_arms().Any()) {
            arm_pair.aim_at(in_enemy.transform);
        }
    }
    public override void on_enemy_disappeared(Intelligence in_enemy) {
        Contract.Requires(in_enemy.GetComponent<Intelligence>() != null);
        arm_pair.handle_target_disappearence(in_enemy);
    }
    
    



    public void aim_at(Transform in_target) {
        arm_pair.aim_at(in_target);
    }
    
    


    
    

    
}

}