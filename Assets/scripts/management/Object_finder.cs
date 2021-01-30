using System.Collections;
using System.Collections.Generic;
using rvinowise.unity.units;
using UnityEngine;
using rvinowise.unity.extensions;

namespace rvinowise.unity.management {
public class Object_finder : MonoBehaviour
{

    public Dictionary_of_lists<Team, Unit> team_to_units = 
        new Dictionary_of_lists<Team, Unit>();


    static public Object_finder instance {
        get; private set;
    } 

    void Awake() {
        contracts.Contract.Requires(instance == null, "it's a singleton");
        instance = this;
    }
    void Start()
    {
        fill_unit_containers();
    }

    private void fill_unit_containers() {
        Unit[] units = FindObjectsOfType<Unit>();
        foreach(Unit unit in units) {
            team_to_units.add(unit.team, unit);
        }
        
    }

    public IReadOnlyList<Unit> get_enemies_of(Unit in_unit) {
        Team in_team = in_unit.team;
        List<Unit> enemies = new List<Unit>();
        foreach(Team enemy_team in in_team.enemies) {
            enemies.AddRange(team_to_units.get_values(enemy_team));
        }
        return enemies;
    }

    public Distance_to_unit get_closest_enemy(Unit unit) {
        Unit closest_enemy = null;
        float closest_distance = float.MaxValue;
        foreach(Unit enemy in get_enemies_of(unit)) {
            float this_distance = unit.transform.position.sqr_distance_to(enemy.transform.position);
            if (this_distance < closest_distance) {
                closest_distance = this_distance;
                closest_enemy = enemy;
            }
        }
        return new Distance_to_unit(closest_enemy, closest_distance);
    }
    
    void Update()
    {
        
    }
    
}

    
public struct Distance_to_unit {
    public Unit unit;
    public float distance;

    public Distance_to_unit(Unit unit, float distance) {
        this.unit = unit;
        this.distance = distance;
    }
}

    
}

