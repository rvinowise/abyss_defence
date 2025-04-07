using System;
using System.Collections;
using System.Collections.Generic;
using rvinowise.unity.actions;
using rvinowise.unity.extensions;
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
    public Arm_pair enemy;
    public float ejection_force = 500;
    
    public List<Combining_circle_ring> rings;

    public Action_runner action_runner;
    
    void Awake() {
        rings = new List<Combining_circle_ring> {
            inner_ring,middle_ring,outer_ring
        };
    }
    

    public void invoke_random_creature(float direction) {

        Combining_circle_invoke_creature.create(
            this,
            direction,
            on_creture_destroyed
        ).set_on_completed(on_rings_reached_direction)
        .start_as_root(action_runner);
    }
    
    [ContextMenu("create_random_combination")]
    void create_random_combination_menu() {
        invoke_random_creature(Random.value * 360);
    }
    
    void invoke_next_creature() {
        var invocation_direction = get_direction_opposite_from_weapons(enemy);
        invoke_random_creature(invocation_direction);
    }
    
    private void FixedUpdate() {
        action_runner.update();
    }

    protected void Update() {
        
    }

    public float get_direction_opposite_from_weapons(Arm_pair arm_pair) {
        var left_targeting = arm_pair.left_tool.transform.rotation;
        var right_targeting = arm_pair.right_tool.transform.rotation;
        var middle_targeting = Quaternion.Slerp(left_targeting, right_targeting , 0.5f);
        var opposite_direction = middle_targeting.to_degree() + new Degree(180);
        return opposite_direction;
    }

    public void on_rings_reached_direction() {
    }

    public void on_creture_destroyed(Intelligence disappeared_unit) {
        invoke_next_creature();
    }


    public bool woken_up = false;
    public void OnTriggerEnter2D(Collider2D other) {
        if (woken_up) {
            return;
        }
        if (other.GetComponentInParent<Intelligence>() is {} other_intelligence) {
            if (other_intelligence.team.is_enemy_team(this.team)) {
                invoke_next_creature();
                woken_up = true;
            }
        }
    }
}

    
}