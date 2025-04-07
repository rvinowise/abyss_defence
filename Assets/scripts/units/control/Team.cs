using System.Collections.Generic;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using rvinowise.unity.extensions;
using UnityEngine;
using UnityEngine.Serialization;


namespace rvinowise.unity {

[Serializable]
public class Team: MonoBehaviour
{
    public List<Team> enemy_teams = new List<Team>();
    public List<Team> ally_teams = new List<Team>();

    private readonly HashSet<Intelligence> units = new HashSet<Intelligence>();
    private readonly HashSet<Targetable> targetables = new HashSet<Targetable>();

    public Color color;

    public int current_ownership_burden;
    public int max_ownership_burden = 400;
    
    
    void Awake() {
        foreach (var unit in FindObjectsByType<Intelligence>(FindObjectsSortMode.None)) {
            if (
                (unit.isActiveAndEnabled)
                &&(unit.team == this)
            ) {
                add_unit(unit);
            }
        }
    }
    
    public void add_unit(Intelligence unit) {
        units.Add(unit);
        unit.team = this;
        current_ownership_burden += unit.ownership_cost;
        if (current_ownership_burden >= max_ownership_burden) {
            Debug.Log($"LIMITING_UNITS: addition of unit {unit.name} exceeded the ownership_burden of team {name}. current_burden = {current_ownership_burden}, max_burden = {max_ownership_burden}");
        }
    }
    
    public void remove_unit(Intelligence unit) {
        if (unit != null) {
            Debug.Log($"LIFETIME: team {name} is removing its unit {unit.name}");
        }
        else {
            Debug.Log($"LIFETIME: team {name} is trying to remove a null unit {unit}");
        }
        units.Remove(unit);
        current_ownership_burden -= unit.ownership_cost;
    }
    
    public void add_targetable(Targetable targetable) {
        targetables.Add(targetable);
        targetable.team = this;
    }
    
    public void remove_targetable(Targetable targetable) {
        targetables.Remove(targetable);
    }

    public IReadOnlyCollection<Intelligence> get_units() {
        return units;
    }

    public IReadOnlyList<Intelligence> get_enemies() {
        List<Intelligence> enemies = new List<Intelligence>();
        foreach (var enemy_team in this.enemy_teams) {
            foreach (var enemy_unit in enemy_team.units) {
                enemies.Add(enemy_unit);
            }
        }
        return enemies.AsReadOnly();
    }
    
    public IReadOnlyList<Transform> get_enemy_transforms() {
        List<Transform> enemies = new List<Transform>();
        foreach (var enemy_team in enemy_teams) {
            foreach (var enemy_unit in enemy_team.units) {
                enemies.Add(enemy_unit.transform);
            }
        }
        return enemies.AsReadOnly();
    }
    
    public IReadOnlyList<Transform> get_enemy_targetables() {
        List<Transform> targetable = new List<Transform>();
        foreach (var enemy_team in enemy_teams) {
            foreach (var enemy_targetable in enemy_team.targetables) {
                if (enemy_targetable) {
                    targetable.Add(enemy_targetable.transform);
                }
                // else {
                //     enemy_team.remove_targetable(enemy_targetable);
                // }
            }
        }
        return targetable;
    }

    public bool is_enemy(GameObject unit) {
        if (unit.GetComponentInChildren<Intelligence>() is {} other_intelligence) {
            return enemy_teams.Exists(team => other_intelligence.team == team);
        }
        return false;
    }
    
    public bool is_enemy_team(Team other_team) {
        return enemy_teams.Contains(other_team);
    }
    
    public List<Tuple<Transform,float>> get_enemies_closest_to(Vector2 in_position) {
        List<Tuple<Transform, float>> enemies_and_distances = new List<Tuple<Transform, float>>();
        foreach (var enemy in get_enemy_transforms()) {
            var distance = enemy.transform.sqr_distance_to(in_position);
            enemies_and_distances.Add(new Tuple<Transform, float>(enemy,distance));
        }
        enemies_and_distances.Sort((tuple1,tuple2) => tuple1.Item2.CompareTo(tuple2.Item2) );
        return enemies_and_distances;
    }

}


}