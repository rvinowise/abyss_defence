﻿using System.Collections.Generic;
using UnityEngine;
using rvinowise.unity.extensions;

namespace rvinowise.unity {
public class Object_finder : MonoBehaviour
{

    public Dictionary_of_lists<Team, Intelligence> team_to_units = 
        new Dictionary_of_lists<Team, Intelligence>();


    static public Object_finder instance {
        get; private set;
    } 

    void Awake() {
        contracts.Contract.Requires(instance == null, "it's a singleton");
        instance = this;
    }

   

    public List<Intelligence> get_enemies_of(Intelligence in_intelligence) {
        Team team = in_intelligence.team;
        List<Intelligence> enemies = new List<Intelligence>();
        if (team != null) { 
            foreach(Team enemy_team in team.enemy_teams) {
                if (team_to_units.get_values(enemy_team) is List<Intelligence> this_enemies) {
                    enemies.AddRange(this_enemies);
                }
            }
        }
        return enemies;
    }

    public Distance_to_component get_closest_enemy(
        Intelligence unit
    ) {
        return get_closest_object(
            unit.transform.position,
            get_enemies_of(unit)
        );
    }

    public Distance_to_component get_closest_object(
        Vector2 position,
        IReadOnlyList<Component> components
    ) {
        Distance_to_component closest = Distance_to_component.empty();
        foreach(Component component in components) {
            float this_distance = position.sqr_distance_to(component.transform.position);
            if (this_distance < closest.distance) {
                closest = new Distance_to_component(component, this_distance);
            }
        }
        return closest;
    }
    
  /*   public Transform get_closest(
        Vector2 position, 
        Type type,

    ) {
        
    } */

    void Update()
    {
        
    }
    
}

public struct Distance_to_component {
    public Component component;
    public float distance;
    public bool exists { get; private set;}

    public Transform get_transform() {
        if (exists) {
            return component.transform;
        }
        return null;
    }

    public static Distance_to_component empty() {
        var distance = new Distance_to_component(null, float.MaxValue) {
            exists = false
        };
        return distance;
    }
    public Distance_to_component(Component component, float distance) {
        this.component = component;
        this.distance = distance;
        this.exists = true;
    }
}


    
}

