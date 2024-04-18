using System;
using System.Collections;
using System.Collections.Generic;
using rvinowise.unity.actions;
using rvinowise.unity.geometry2d;
using UnityEngine;
using Random = UnityEngine.Random;


namespace rvinowise.unity {
public class Combining_circle : MonoBehaviour {

    public Combining_circle_ring inner_ring;
    public Combining_circle_ring middle_ring;
    public Combining_circle_ring outer_ring;
    public Combining_circle_wall wall;
    public Team team;

    public List<Combining_circle_ring> rings;

    private Action_runner action_runner = new Action_runner();
    
    void Awake() {
        rings = new List<Combining_circle_ring> {
            inner_ring,middle_ring,outer_ring
        };
    }
    

    public void invoke_random_creature() {

        Combining_circle_invoke_creature.create(
            this
        ).set_on_completed(on_rings_reached_direction)
        .start_as_root(action_runner);
    }
    
    [ContextMenu("create_random_combination")]
    void create_random_combination_menu() {
        invoke_random_creature();
    }
    
    private void FixedUpdate() {
        action_runner.update();
    }

    public void on_rings_reached_direction() {
    }


    
}

    
}