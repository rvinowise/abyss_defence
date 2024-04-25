using System;
using System.Globalization;
using System.Linq;
using rvinowise.unity.actions;
using rvinowise.unity.extensions;
using UnityEditor;
using UnityEngine;
using Action = rvinowise.unity.actions.Action;


namespace rvinowise.unity {


public enum Intelligence_action: int {
    Starting_defending,
    Finishing_defending,
    Defending,
    Attacking,
    Walking,
    Nothing
}
public class Computer_intelligence:Intelligence {
    
    public readonly Unit_commands unit_commands = new Unit_commands();
    public Commander commander;

    [NonSerialized]
    public Intelligence target;

    private Intelligence_action intelligence_action;
    
    public override void Awake() {
        base.Awake();
        set_team(team);
    }

    public void set_team(Team in_team) {
        if (in_team == null) {
            return;
        }
        team = in_team;
        var sprite_renderers = this.GetComponentsInChildren<SpriteRenderer>();
        foreach (var sprite_renderer in sprite_renderers) {
            var old_color = sprite_renderer.color;
            sprite_renderer.color = team.color * old_color;// new Color(old_color.r, old_color.g, old_color.b);
        }
    }
    
    protected override void Start() {
        base.Start();

        move_towards_best_target();
    }

    
    
    public void move_towards_best_target() {
        target = find_best_target();
        if (target != null) {
            target.on_destroyed += on_target_disappeared;

            move_towards_target(target);
        }
        else {
            Idle.create(transporter).start_as_root(action_runner);
        }
    }

    private void move_towards_target(Intelligence in_target) {
        intelligence_action = Intelligence_action.Walking;
        Action_parallel_parent.create(
            Keep_distance_from_target.create(
                transporter,
                transform,
                get_body_reaching_distance(),
                in_target.transform
            ),
            Sensor_pay_attention_to_target.create(
                sensory_organ,
                in_target.transform
            )
        )
        .set_on_completed(on_reached_destination)
        .start_as_root(action_runner);
    }

    private float get_body_reaching_distance() {
        var weapon_reach = attacker.get_reaching_distance();
        //var weapon_relative_position =
        //    transform.InverseTransformPoint(attacker.transform.position);
        return weapon_reach + 0.1f;
    }

    private void on_reached_destination(Action moving_action) {
        Debug.Log($"action {moving_action.get_explanation()} has reached destination");
        move_towards_target(target);
    }

    protected void Update() {
        action_runner.update();
    }
    
    protected void FixedUpdate() {
        if (target != null) {
            if (intelligence_action == Intelligence_action.Walking) {
                if (attacker.can_reach(target.transform)) 
                {
                    Debug.unityLogger.Log("ATTACK_DEFENCE", $"Computer_intelligence::FixedUpdate: weaponry.attack({target.name})");
                    intelligence_action = Intelligence_action.Attacking;
                    attacker.attack(target.transform, on_attack_finished);
                } else
                if (target.attacker.can_reach(this.transform)) {
                    Debug.unityLogger.Log("ATTACK_DEFENCE", $"Computer_intelligence::FixedUpdate: target.weaponry.can_reach this, defending");
                    intelligence_action = Intelligence_action.Starting_defending;
                    defender.start_defence(target.transform, on_assumed_defensive_position);
                }
            }
            else {
                if (
                    (
                        intelligence_action == Intelligence_action.Defending
                        ||
                        intelligence_action == Intelligence_action.Starting_defending
                    )
                    &&
                    (!target.attacker.can_reach(this.transform))
                ) 
                {
                    Debug.unityLogger.Log("ATTACK_DEFENCE", $"Computer_intelligence::FixedUpdate: !target.weaponry.can_reach this, walking");
                    intelligence_action = Intelligence_action.Finishing_defending;
                    defender.finish_defence(on_finished_defensive_position);
                } 
                else if 
                (
                    intelligence_action == Intelligence_action.Finishing_defending
                    &&
                    (target.attacker.can_reach(this.transform))
                ) 
                {
                    Debug.unityLogger.Log("ATTACK_DEFENCE", $"Keep defending, don't end the defence");
                    intelligence_action = Intelligence_action.Starting_defending;
                    defender.start_defence(target.transform, on_assumed_defensive_position);
                }
            }
        }
    }

    public override void on_enemy_appeared(Intelligence in_enemy) {
        if (target == null) {
            target = in_enemy;
            target.on_destroyed += on_target_disappeared;
            move_towards_target(target);
        }
    }


    private Intelligence find_closest_enemy() {
        if (team == null) {
            return null;
        }
        var closest_distance = float.MaxValue;
        Intelligence closest_enemy = null;
        foreach (var enemy_team in team.enemy_teams) {
            foreach (var enemy in enemy_team.units) {
                if (enemy == null) {
                    Debug.LogError($"enemy of {name} from team {enemy_team.name} is null");
                }
                var this_distance = transform.sqr_distance_to(enemy.transform.position);
                if (this_distance < closest_distance) {
                    closest_distance = this_distance;
                    closest_enemy = enemy;
                }
            }
        }
        return closest_enemy;
    }
    private Intelligence find_closest_friend() {
        var closest_distance = float.MaxValue;
        Intelligence closest_unit = null;
        foreach (var unit in team.units) {
            if (unit==this)
                continue;
            var this_distance = transform.sqr_distance_to(unit.transform.position);
            if (this_distance < closest_distance) {
                closest_distance = this_distance;
                closest_unit = unit;
            }
        }
        return closest_unit;
    }
    
    private Intelligence find_best_target() {
        return find_closest_enemy();
    }

    private void on_target_disappeared(Intelligence disappearing_unit) {
        if (this == null) return;
        
        move_towards_best_target();
    }

    private void on_attack_finished() {
        if (this == null) return;
        
        intelligence_action = Intelligence_action.Walking;
        move_towards_best_target();
    }
    
    private void on_assumed_defensive_position() {
        if (this == null) return;
        
        Debug.unityLogger.Log("ATTACK_DEFENCE", "Defending state");
        
        intelligence_action = Intelligence_action.Defending;
    }

    private void on_finished_defensive_position() {
        if (this == null) return;
        
        Debug.unityLogger.Log("ATTACK_DEFENCE", "on_finished_defensive_position, Walking stage");
        intelligence_action = Intelligence_action.Walking;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos() {
        Handles.color = Color.green;
        
        GUIStyle myStyle = new GUIStyle {
            fontSize = 20,
            fontStyle = FontStyle.Bold
        };
        myStyle.normal.textColor = Color.black;

        Handles.Label(transform.position, intelligence_action.ToString(),myStyle);

        if (GetComponent<Damage_receiver>() is { } damage_receiver) {
            myStyle.normal.textColor = Color.green;
            Handles.Label(transform.position, damage_receiver.received_damage.ToString(),myStyle);
        }
    }
#endif
    
    void OnTriggerEnter2D(Collider2D other) {
        // if (other.GetComponent<Damage_receiver>() is Damage_receiver damage_receiver) {
        //     damage_receiver.receive_damage();
        // }
        Debug.Log("trigger enter test");
    }

    private void OnTriggerExit2D(Collider2D other) {
        Debug.Log("trigger exit test");
    }
    
}
}