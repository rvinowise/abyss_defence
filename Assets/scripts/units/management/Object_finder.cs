using System.Collections;
using System.Collections.Generic;
using rvinowise.unity.units;
using UnityEngine;
using rvinowise.unity.extensions;
using System;
using rvinowise.unity.units.control;

namespace rvinowise.unity.management {
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
    void Start()
    {
    }

   

    public List<Intelligence> get_enemies_of(Intelligence in_intelligence) {
        Team team = in_intelligence.team;
        List<Intelligence> enemies = new List<Intelligence>();
        if (team != null) { 
            foreach(Team enemy_team in team.enemies) {
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
            get_enemies_of(unit) as IReadOnlyList<Component>
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

    public Transform get_transform() {
        if (component != null) {
            return component.transform;
        }
        return null;
    }

    public static Distance_to_component empty() {
        return new Distance_to_component(null, float.MaxValue);
    }
    public Distance_to_component(Component component, float distance) {
        this.component = component;
        this.distance = distance;
    }
}

public struct Distance_to_transform {
    public Transform transform;
    public float distance;

    public static Distance_to_transform empty() {
        return new Distance_to_transform(null, float.MaxValue);
    }
    public Distance_to_transform(Transform transform, float distance) {
        this.transform = transform;
        this.distance = distance;
    }
}

    
}

