using System.Collections.Generic;
using System;
using rvinowise.unity.extensions;
using UnityEngine;
using UnityEngine.Serialization;


namespace rvinowise.unity {

[Serializable]
public class Team: MonoBehaviour
{
    public List<Team> enemy_teams = new List<Team>();
    public List<Team> ally_teams = new List<Team>();

    public HashSet<Intelligence> units = new HashSet<Intelligence>();

    public Color color;
    
    void Awake() {
        foreach (var unit in FindObjectsByType<Intelligence>(FindObjectsSortMode.None)) {
            if (unit.team == this) {
                add_unit(unit);
            }
        }
    }
    
    public void add_unit(Intelligence unit) {
        units.Add(unit);
        unit.team = this;
    }
    
    public void remove_unit(Intelligence unit) {
        if (unit != null) {
            Debug.Log($"LIFETIME: team {name} is removing its unit {unit.name}");
        }
        else {
            Debug.Log($"LIFETIME: team {name} is trying to remove a null unit {unit}");
        }
        units.Remove(unit);
    }

    public List<Intelligence> get_enemies() {
        List<Intelligence> enemies = new List<Intelligence>();
        foreach (var enemy_team in this.enemy_teams) {
            foreach (var enemy_unit in enemy_team.units) {
                enemies.Add(enemy_unit);
            }
        }
        return enemies;
    }
    
    public List<Transform> get_enemy_transforms() {
        List<Transform> enemies = new List<Transform>();
        foreach (var enemy_team in this.enemy_teams) {
            foreach (var enemy_unit in enemy_team.units) {
                enemies.Add(enemy_unit.transform);
            }
        }
        return enemies;
    }

    public bool is_enemy(GameObject unit) {
        if (unit.GetComponentInChildren<Intelligence>() is {} other_intelligence) {
            return enemy_teams.Exists(team => other_intelligence.team == team);
        }
        return false;
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