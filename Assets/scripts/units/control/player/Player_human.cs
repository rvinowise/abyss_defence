using UnityEngine;
using rvinowise.unity.extensions;

using static rvinowise.unity.geometry2d.Directions;
using rvinowise.contracts;
using System.Linq;
using rvinowise.unity.actions;
using rvinowise.unity.geometry2d;

namespace rvinowise.unity {

public abstract class Player_human : 
    Human_intelligence
    ,IInput_receiver
{

    private int[] held_tool_index;

    protected override void Start() {
        base.Start();
        find_and_assign_team();
        consider_all_enemies();
        sensory_organ?.pay_attention_to_target(Player_input.instance.cursor.transform);
        Player_input.instance.add_input_receiver(this);
    }

    private void find_and_assign_team() {
        GameObject.FindWithTag("player team")?.GetComponent<Team>().add_unit(this);
    }

    private void consider_all_enemies() {
        foreach(Team enemy_team in team.enemy_teams) {
            foreach(var enemy_unit in enemy_team.get_units()) {
                on_enemy_appeared(enemy_unit);
            }
        }
    }

    
    public bool is_finished { get; set; }

    public bool process_input() {
        var is_input_used = false;
        is_input_used |= move_in_space();
        is_input_used |= maybe_switch_items();
        is_input_used |= use_tools();
        return is_input_used;
    }

    protected void Update() {
        action_runner.update();
    }

    protected abstract bool use_tools();

    public Transform get_selected_target() { // out of the two targets of both hands
        Distance_to_component closest = Distance_to_component.empty();
        foreach(Transform target in Arm_pair_aiming.get_all_targets(arm_pair)) {
            float this_distance = target.sqr_distance_to(Player_input.instance.cursor.transform.position);
            if (this_distance < closest.distance) {
                closest = new Distance_to_component(target, this_distance);
            }
        }
        return closest.component as Transform;
    }
    

    protected abstract bool maybe_switch_items();

    public Arm get_selected_arm() {
        return Arm_pair_helpers.get_arm_on_side(arm_pair, get_selected_side());
    }
    
    public float last_rotation;

    public Side_type get_selected_side() {
        return Side.from_degrees(last_rotation);
    }
    protected void save_last_rotation(Quaternion needed_direction) {
        float angle_difference = transform.rotation.degrees_to(needed_direction).degrees;
        last_rotation = angle_difference;
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

    private bool move_in_space() {
        if (transporter == null) {
            return false;
        }
        var destination = (Vector2)transform.position+Player_input.instance.moving_vector;
        transporter.move_towards_destination(destination);
        
        Vector2 mouse_pos = Player_input.instance.cursor.transform.position;
        Quaternion needed_direction = (mouse_pos - (Vector2) transform.position).to_quaternion();
        transporter.face_rotation(needed_direction);

        if (Player_input.instance.moving_vector.magnitude > 0) {
            return true;
        }
        return false;
    }

    // private Gun has_gun_in_2hands() {
    //     if (
    //         (arm_pair?.right_arm.current_action is Idle_vigilant_main_arm) &&
    //         (arm_pair?.right_arm.held_tool is { } tool)
    //     ) {
    //         return tool.GetComponent<Gun>();
    //     }
    //     return null;
    // }

    


    
}

}