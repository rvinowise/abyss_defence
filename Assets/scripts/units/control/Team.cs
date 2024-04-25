using System.Collections.Generic;
using System;
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
        Debug.Log($"LIFETIME: team {name} is removing its unit {unit.name}");
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
    
    public List<Transform> get_enemiy_transforms() {
        List<Transform> enemies = new List<Transform>();
        foreach (var enemy_team in this.enemy_teams) {
            foreach (var enemy_unit in enemy_team.units) {
                enemies.Add(enemy_unit.transform);
            }
        }
        return enemies;
    }

}


}