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

public class Jumping_leg_group: 
    Creeping_leg_group {


    public List<ALeg> jumping_legs = new List<ALeg>();

    public float jump_cooldown = 3;
    public float jump_force = 300;
    private float time_of_next_jump;
    internal override void Awake() {
        base.Awake();
    }

    void jump() {
        time_of_next_jump = Time.time + jump_cooldown;
        rigid_body.AddForce(moved_body.rotation.to_vector()*jump_force,ForceMode2D.Impulse);
    }

    bool can_jump() {
        return
            (jumping_cooldown_elapsed())
            &&
            (jumping_legs_are_standing())
            &&
            (jumping_legs_can_push());
    }

    bool jumping_cooldown_elapsed() {
        return Time.time > time_of_next_jump;
    }

    bool jumping_legs_are_standing() {
        foreach (var leg in jumping_legs) {
            if (leg.is_up()) {
                return false;
            }
        }
        return true;
    }

    bool jumping_legs_can_push() {
        return true;
    }
    
    bool is_facing_target(Vector2 target) {
        var jumping_direction = moved_body.transform.rotation;
        var direction_to_target = (target - (Vector2) moved_body.transform.position).to_dergees();
        return
            jumping_direction.to_degree().angle_to(direction_to_target) < 10;
    }
    
    protected override void Update() {
        base.Update();
        
    }
    
    public override void move_towards_destination(Vector2 destination) {
        moving_vector = (destination - (Vector2) transform.position).normalized;
        if (
            (can_jump())
            &&
            (is_facing_target(destination))
        ) {
            jump();
        }
        else {
            move_in_direction(moving_vector);
        }
    }
}

}

