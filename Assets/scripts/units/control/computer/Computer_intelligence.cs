//#define RVI_DEBUG

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Pathfinding;
using rvinowise.unity.actions;
using rvinowise.unity.extensions;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions.Comparers;
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

    public Seeker path_seeker;
    
    public readonly Unit_commands unit_commands = new Unit_commands();
    public Intelligence_action intelligence_action;
    
    [NonSerialized]
    public Damage_receiver target;
    
    #if RVI_DEBUG
    public static int counter = 0;
    public int number = 0;
    #endif
    
    public override void Awake() {
        base.Awake();
        set_team(team);
        path_seeker = GetComponent<Seeker>();
        
#if RVI_DEBUG
    int number = counter++;
    action_runner.number = number;
#endif
    }

    public void set_team(Team in_team) {
        if (in_team == null) {
            return;
        }
        team = in_team;
        var sprite_renderers = GetComponentsInChildren<SpriteRenderer>();
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
#if RVI_DEBUG
        Debug.Log($"COMPUTER {name} #{number} Computer_intelligence::move_towards_best_target");
#endif
        target = find_best_target();
        if (target != null) {
            target.on_destroyed += on_target_disappeared;

            move_towards_target(target.transform);
        }
        else {
            Idle.create(transporter.actor).start_as_root(action_runner);
        }
    }

    private void move_towards_target(Transform in_target) {
#if RVI_DEBUG
        Debug.Log($"COMPUTER {name} #{number} Computer_intelligence::move_towards_target({in_target})");
#endif
        intelligence_action = Intelligence_action.Walking;
        Action_parallel_parent.create(
            Follow_target.create(
                transporter,
                transform,
                path_seeker,
                in_target,
                get_body_reaching_distance()
            ),
            Sensor_pay_attention_to_target.create(
                sensory_organ,
                in_target
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
#if RVI_DEBUG
        Debug.Log($"COMPUTER {name} #{number} Computer_intelligence::on_reached_destination(moving_action={moving_action})");
#endif
        move_towards_target(target.transform);
    }

    protected void Update() {
        if (gameObject.name == "test") {
            bool test = true;
        }
        action_runner.update();
    }

    private Damage_receiver get_best_target(IList<Damage_receiver> targets) {
        if (targets.Contains(target)) {
            return target;
        }
        return targets.First();
    }
    
    protected void FixedUpdate() {
        if (gameObject.name == "test") {
            bool test = true;
        }
        if (target != null) {
            sensory_organ.pay_attention_to_target(target.transform);
            
            if (intelligence_action == Intelligence_action.Walking) {
                var reachable_targets = attacker.get_targets().ToList();
                if (reachable_targets.Any()) {
                    target = get_best_target(reachable_targets);
                    
#if RVI_DEBUG
                    Debug.Log($"COMPUTER {name} #{number} Computer_intelligence::FixedUpdate: weaponry.attack({target.name})");
#endif
                    intelligence_action = Intelligence_action.Attacking;
                    attacker.attack(target.transform, on_attack_finished);
                }
//                 else
//                 if 
//                     (
//                         (target.attacker.is_weapon_ready_for_target(this.transform))
//                         &&
//                         !(defender is Empty_defender)
//                     )
//                 {
// #if RVI_DEBUG
//                     Debug.Log($"COMPUTER {name} #{number} Computer_intelligence::FixedUpdate: target.weaponry.can_reach this, defending");
// #endif
//                     intelligence_action = Intelligence_action.Starting_defending;
//                     defender.start_defence(target.transform, on_assumed_defensive_position);
//                 }
//             }
//             else {
//                 if (
//                     (
//                         intelligence_action == Intelligence_action.Defending
//                         ||
//                         intelligence_action == Intelligence_action.Starting_defending
//                     )
//                     &&
//                     (!target.attacker.is_weapon_ready_for_target(this.transform))
//                 ) 
//                 {
// #if RVI_DEBUG
//                     Debug.Log($"COMPUTER {name} #{number} Computer_intelligence::FixedUpdate: !target.weaponry.can_reach this, walking");
// #endif
//                     intelligence_action = Intelligence_action.Finishing_defending;
//                     defender.finish_defence(on_finished_defensive_position);
//                 } 
//                 else if 
//                 (
//                     intelligence_action == Intelligence_action.Finishing_defending
//                     &&
//                     (target.attacker.is_weapon_ready_for_target(this.transform))
//                 ) 
//                 {
// #if RVI_DEBUG
//                     Debug.Log($"COMPUTER {name} #{number} Keep defending, don't end the defence");
// #endif
//                     intelligence_action = Intelligence_action.Starting_defending;
//                     defender.start_defence(target.transform, on_assumed_defensive_position);
//                 }
            }
        }
    }

    public override void on_enemy_appeared(Intelligence in_enemy) {
#if RVI_DEBUG
        Debug.Log($"COMPUTER {name} #{number} Computer_intelligence::on_enemy_appeared({in_enemy})");
#endif

        if (target == null) {
            target = find_closest_damageable_of_unit(in_enemy);
            target.on_destroyed += on_target_disappeared;
            move_towards_target(target.transform);
        }
    }


    private Intelligence find_closest_enemy() {
        if (team == null) {
            return null;
        }
        var closest_distance = float.PositiveInfinity;
        Intelligence closest_enemy = null;
        foreach (var enemy_team in team.enemy_teams) {
            foreach (var enemy in enemy_team.units) {
                if (enemy == null) {
                    Debug.LogError($"enemy of {name} from team {enemy_team.name} is null");
                }
                if (!enemy.is_ignored) {
                    var this_distance = transform.sqr_distance_to(enemy.transform.position);
                    if (this_distance < closest_distance) {
                        closest_distance = this_distance;
                        closest_enemy = enemy;
                    }
                }
            }
        }
        return closest_enemy;
    }

    public static TComponent find_closest_component<TComponent>(Vector2 position, IEnumerable<TComponent> components) 
    where TComponent: Component {
        float closest_distance = float.PositiveInfinity;
        TComponent closest_component = null;
        foreach (var component in components) {
            var this_distance = (position - (Vector2)component.transform.position).sqrMagnitude;
            if (this_distance < closest_distance) {
                closest_distance = this_distance;
                closest_component = component;
            }
        }
        return closest_component;
    }

    private Damage_receiver find_closest_damageable_of_unit(Intelligence unit) {
        if (unit == null)
        {
            return null;
        }
        var damageables = unit.GetComponentsInChildren<Damage_receiver>();
        return find_closest_component(transform.position, damageables);

    }
    
    private Intelligence find_closest_friend() {
        var closest_distance = float.PositiveInfinity;
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
    
    private Damage_receiver find_best_target() {
        var closest_enemy = find_closest_enemy();
        return find_closest_damageable_of_unit(closest_enemy);
    }

    private void on_target_disappeared(Damage_receiver disappearing_target) {
        if (this == null) return;
#if RVI_DEBUG
        Debug.Log($"COMPUTER {name} #{number} Computer_intelligence::on_target_disappeared({disappearing_unit})");
#endif
        move_towards_best_target();
    }

    private void on_attack_finished() {
        if (this == null) return;
#if RVI_DEBUG
        Debug.Log($"COMPUTER {name} #{number} Computer_intelligence::on_attack_finished");
#endif        

        intelligence_action = Intelligence_action.Walking;
        move_towards_best_target();
    }
    
    private void on_assumed_defensive_position() {
        if (this == null) return;
        
#if RVI_DEBUG
        Debug.Log($"COMPUTER {name} #{number} Computer_intelligence::on_assumed_defensive_position");
#endif
        
        intelligence_action = Intelligence_action.Defending;
    }

    private void on_finished_defensive_position() {
        if (this == null) return;
        
#if RVI_DEBUG
        Debug.Log($"COMPUTER {name} #{number} Computer_intelligence::on_finished_defensive_position");
#endif
        intelligence_action = Intelligence_action.Walking;
        move_towards_best_target();
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

        draw_targeting();
    }

    private void draw_targeting() {
        if (target != null) {
            
            //Handles.
            var start_point = attacker.actor == null ? transform.position : attacker.actor.transform.position;
            var end_point = target.transform.position;

            Vector3 middle_point =
                start_point
                +
                (end_point - start_point) / 2;
                
            Handles.color = new Color(1f,0.6f,0f);
            Handles.DrawLine(start_point, end_point,1);
            Handles.color = new Color(1f,0.2f,0f);
            Handles.DrawLine(start_point, middle_point,2);
        }
    }
    
#endif
    
    void OnTriggerEnter2D(Collider2D other) {
        // if (other.GetComponent<Damage_receiver>() is Damage_receiver damage_receiver) {
        //     damage_receiver.receive_damage();
        // }
        //Debug.Log("trigger enter test");
    }

    private void OnTriggerExit2D(Collider2D other) {
        //Debug.Log("trigger exit test");
    }
    
}
}