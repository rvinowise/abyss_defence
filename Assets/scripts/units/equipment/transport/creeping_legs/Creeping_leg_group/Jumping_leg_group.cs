using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using rvinowise.contracts;
using rvinowise.unity.actions;
using rvinowise.unity.extensions;
using rvinowise.unity.geometry2d;
using UnityEngine.Serialization;
using Action = rvinowise.unity.actions.Action;
using Random = UnityEngine.Random;


namespace rvinowise.unity {

public class Jumping_leg_group: 
    Creeping_leg_group {


    public List<ALeg> jumping_legs = new List<ALeg>();

    public float jump_cooldown = 3;
    public float leg_jump_force = 300;
    private float time_of_next_jump;
    protected override void Awake() {
        base.Awake();
    }

    void jump() {
        time_of_next_jump = Time.time + jump_cooldown;
        rigid_body.AddForce(moved_body.rotation.to_vector()*get_jumping_forse(),ForceMode2D.Impulse);
    }

    public float get_jumping_forse() {
        return jumping_legs.Count * leg_jump_force;
    }
    
    bool can_jump() {
        if (!jumping_legs.Any()) {
            return false;
        }
        
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
            Math.Abs(jumping_direction.to_degree().angle_to(direction_to_target)) < 10;
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

    public override void distribute_data_across(
        IEnumerable<IChildren_group> new_controllers
    ) {
        
        List<Creeping_leg_group> new_leg_controllers = 
            new_controllers.Cast<Creeping_leg_group>().ToList();

        foreach (var leg_controller in new_leg_controllers) {
            Contract.Requires(leg_controller != null);    
        }
        
        Creeping_leg_group_distributor.distribute_data_across(this,new_leg_controllers);

        distribute_jumping_legs(new_leg_controllers);
    }

    public void distribute_jumping_legs(IEnumerable<IChildren_group> new_controllers) {
        var distributed_jumping_legs = jumping_legs;
        foreach (var child_controller in new_controllers) {
            if (child_controller is Jumping_leg_group new_jumping_group) {
                new_jumping_group.jumping_legs = new List<ALeg>();
                foreach (var new_leg in new_jumping_group.legs) {
                    if (distributed_jumping_legs.Contains(new_leg)) {
                        new_jumping_group.jumping_legs.Add(new_leg);
                    }
                }
                
            }
            else {
                Debug.LogError($"the children group, which is split by jumping legs {this.name}, is not a jumping leg group");
            }
        }
    }
}

}

