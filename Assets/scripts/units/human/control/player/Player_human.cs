using System.Collections.Generic;
using UnityEngine;
using rvinowise.unity.extensions;

using rvinowise.unity.units.parts.limbs.arms;
using rvinowise.unity.units.parts.limbs.arms.actions.idle_vigilant.main_arm;
using rvinowise.unity.units.parts.weapons.guns;
using static rvinowise.unity.geometry2d.Directions;
using Player_input = rvinowise.unity.ui.input.Player_input;
using rvinowise.unity.units.parts.limbs.arms.humanoid;
using rvinowise.unity.management;
using rvinowise.contracts;
using System.Linq;
using rvinowise.unity.geometry2d;

namespace rvinowise.unity.units.control.human {

public abstract class Player_human : Human_intelligence {

    private int[] held_tool_index;
    private Unit unit;
    private Transform cursor_transform;

    public List<Transform> enemies = new List<Transform>();

    protected override void Awake() {
        base.Awake();
        unit = GetComponent<Unit>();
        arm_pair = GetComponent<Arm_pair>();
    }

    protected override void Start() {
        base.Start();
        cursor_transform = Player_input.instance.cursor.transform;
        arm_pair.on_target_disappeared += find_new_target;
        consider_all_enemies();
    }

    private void consider_all_enemies() {
        foreach(Team enemy_team in team.enemies) {
            foreach(var enemy_unit in enemy_team.units) {
                consider_enemy(enemy_unit);
            }
        }
    }

    


    protected override void read_input() {
        read_transporter_input();
        read_switching_items_input();
        read_sensory_organs_input();
        read_using_tools_input();
    }


    protected abstract void read_using_tools_input();

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

    private void idle(Arm arm) {
        var direction_to_mouse = transform.quaternion_to(Player_input.instance.mouse_world_position);
        arm.upper_arm.target_rotation =
            arm.upper_arm.desired_idle_rotation * direction_to_mouse;
        
        arm.forearm.target_rotation =
            arm.forearm.desired_idle_rotation * direction_to_mouse;
        arm.hand.target_rotation =
            arm.hand.desired_idle_rotation * direction_to_mouse;
    }

    private void read_sensory_organs_input() {
        sensory_organ?.pay_attention_to(Player_input.instance.mouse_world_position);
    }

    protected abstract void read_switching_items_input();

    public Arm get_selected_arm() {
        return arm_pair.get_arm_on_side(get_selected_side());
    }
    
    public Side_type get_selected_side() {
        return Side.from_degrees(last_rotation);
    }


    /*private bool wants_to_switch_tool() {
        return (switching_items_is_possible() && )
    }*/
    protected bool switching_items_is_possible() {
        if (baggage == null) {
            return false;
        }
        if (Player_input.instance.zoom_held) {
            return false;
        }
        return true;
    }

    private void read_transporter_input() {
        if (transporter == null) {
            return;
        }
        transporter.command_batch.moving_direction_vector = read_moving_direction();
        transporter.command_batch.face_direction_quaternion = read_face_direction();

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


    public override void consider_enemy(Intelligence in_enemy) {
        Contract.Requires(enemies.IndexOf(in_enemy.transform) == -1, "adding enemy the second time");
        enemies.Add(in_enemy.transform);
        subscribe_to_disappearance_of(in_enemy.transform);
        if (arm_pair.get_iddling_armed_autoaimed_arms().Any()) {
            arm_pair.aim_at(in_enemy.transform);
        }
    }
    public void on_enemy_disappeared(Damage_receiver in_enemy) {
        Contract.Requires(in_enemy.GetComponent<Intelligence>() != null);
        enemies.Remove(in_enemy.GetComponent<Transform>());
    }
    
    public void find_new_target(Arm in_arm) {
        List<Transform> free_enemies = get_not_targeted_enemies();
        Distance_to_component closest_target = Object_finder.instance.get_closest_object(
            cursor_transform.position,
            free_enemies as IReadOnlyList<Component>
        );
        if (closest_target.get_transform() != null) {
            arm_pair.set_target_for(in_arm, closest_target.get_transform());
        }
    }
    private List<Transform> get_not_targeted_enemies() {
        return enemies.Except(arm_pair.get_all_targets()).ToList();
    }



    public void aim_at(Transform in_target) {
        arm_pair.aim_at(in_target);
    }
    
    

    private void subscribe_to_disappearance_of(Transform in_unit) {
        if (in_unit.GetComponent<Damage_receiver>() is Damage_receiver damage_receiver) {
            damage_receiver.on_destroyed+=on_enemy_disappeared;
        }
    }

    

    
    

    
}

}