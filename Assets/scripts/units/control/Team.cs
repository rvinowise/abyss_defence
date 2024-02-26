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

    void Awake() {
        foreach (var unit in FindObjectsByType<Intelligence>(FindObjectsSortMode.None)) {
            if (unit.team == this) {
                add_unit(unit);
            }
        }
    }
    
    public void add_unit(Intelligence unit) {
        units.Add(unit);
    }
    
    public void remove_unit(Intelligence unit) {
        units.Remove(unit);
    }

}


}