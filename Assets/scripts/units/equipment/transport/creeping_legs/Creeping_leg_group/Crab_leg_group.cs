using System;
using System.Collections.Generic;
using UnityEngine;

using rvinowise.contracts;
using rvinowise.unity.actions;
using rvinowise.unity.extensions;
using rvinowise.unity.geometry2d;
using Action = rvinowise.unity.actions.Action;
using Random = UnityEngine.Random;


namespace rvinowise.unity {

public class Crab_leg_group: 
    Creeping_leg_group {

    public float min_direction_change_time = 0.5f;
    public float max_direction_change_time = 5f;
    public float degrees_side_from_target = 80f;
    
    protected Side_type circumventing_side = Side_type.LEFT;

    private float time_of_next_side_change;
    protected override void Awake() {
        base.Awake();
        change_circumventing_side();
    }

    void change_circumventing_side() {
        circumventing_side = (Side_type) (-(int)circumventing_side);
        time_of_next_side_change = Time.time + Random.Range(min_direction_change_time, max_direction_change_time);
    }
    
    protected override void Update() {
        base.Update();
        if (Time.time >= time_of_next_side_change) {
            change_circumventing_side();
        }
    }
    
    public override void move_towards_destination(Vector2 destination) {
        var vector_towards_target = (destination - (Vector2) transform.position).normalized;

        moving_vector = vector_towards_target.rotate(Side.turn_degrees(circumventing_side, degrees_side_from_target));
        
        move_in_direction(moving_vector);
    }
}

}

