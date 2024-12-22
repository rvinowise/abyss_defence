using System;
using System.Collections.Generic;
using System.Linq;
using rvinowise.unity.geometry2d;
using UnityEngine;
using UnityEngine.Serialization;


namespace rvinowise.unity {


public class Landmine: MonoBehaviour {

    public Explosive_body explosive_body;
    public Collider2D trigger_collider;
    
    public Team team;
    public SpriteRenderer sprite_renderer;

    public void Start() {
        if (team != null) {
            assign_to_team(team);
        }
    }

    public void install(Team owner_team) {
        Debug.Assert(trigger_collider.enabled == false, "a landmine can't be enabled twice");
        
        assign_to_team(team);
        
        trigger_collider.enabled = true;
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (team.is_enemy(other.gameObject)) {
            explosive_body.on_start_dying();
        }
    }

    private void assign_to_team(Team owner_team) {
        team = owner_team;
        
        sprite_renderer.material.color = owner_team.color;
        sprite_renderer.color = owner_team.color;
    }

}

}