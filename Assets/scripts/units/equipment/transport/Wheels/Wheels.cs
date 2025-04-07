using System;
using System.Collections.Generic;
using System.Linq;
using rvinowise.unity.extensions;
using rvinowise.unity.geometry2d;
using rvinowise.unity.actions;
using UnityEngine;
using UnityEngine.Serialization;
using Action = rvinowise.unity.actions.Action;


namespace rvinowise.unity {

public class Wheels:
    MonoBehaviour
    ,ITransporter
{

    public Turning_element moved_body;

    public void set_moved_body(Turning_element body) {
        moved_body = body;
    }
    public Turning_element get_moved_body() {
        return moved_body;
    }

    [FormerlySerializedAs("turning_wheels")] public List<Wheel> steering_wheels;
    public List<Wheel> static_wheels;

    public Actor actor { get; set; }

    public void on_lacking_action() {
        Idle.create(this).start_as_root(actor.action_runner);
    }

   

    public float rotation_speed = 100f;
    public float acceleration_speed = 1f;
    public float wheels_turning_speed = 10f;
    public float wheels_turning_amplitude = 45f;
    public float get_possible_rotation() {
        return rotation_speed;
    }
    public float get_possible_impulse() {
        return acceleration_speed;
    }


    public Rigidbody2D moved_rigid_body;
    void Awake() {
        actor = GetComponent<Actor>();
    }

    private Degree get_steering_wheels_angle() {
        return transform.rotation.to_degree().angle_to(steering_wheels.First().transform.rotation);
    }
    
    private void turn_steering_wheels(float angle_change) {
        foreach(var steering_wheel in steering_wheels) {
            steering_wheel.transform.rotation *= new Degree(angle_change).to_quaternion();
        }
    }

    private void fix_steering_wheels_at_angle(Degree angle) {
        foreach(var steering_wheel in steering_wheels) {
            steering_wheel.transform.rotation = angle.to_quaternion();
        }
    }

    private void keep_steering_wheels_within_amplitude() {
        var difference = transform.rotation.to_degree().angle_to(get_steering_wheels_angle());
        if (Math.Abs(difference) > wheels_turning_amplitude) {
            fix_steering_wheels_at_angle(wheels_turning_amplitude*Math.Sign(difference));
        }
    }
    
    private void turn_steering_wheels_towards_relative_angle(Degree destination_angle) {
        var difference = get_steering_wheels_angle().angle_to(destination_angle);
        if (wheels_turning_speed*Time.deltaTime <= difference) {
            fix_steering_wheels_at_angle(destination_angle);
        } else {
            turn_steering_wheels(wheels_turning_speed * Math.Abs(difference) * Time.deltaTime);
        }
        keep_steering_wheels_within_amplitude();
    }

    public void move_towards_destination(Vector2 destination) {
        var absolute_rotation_to_destination = transform.position.quaternion_to(destination);
        var needed_speed_ratio = (180-Quaternion.Angle(transform.rotation, absolute_rotation_to_destination)) / 180;

        var current_speed_forward = moved_rigid_body.velocity.magnitude;

        var force_vector = transform.rotation * get_steering_wheels_angle() * Vector2.right * acceleration_speed *Time.deltaTime;
        if (current_speed_forward < needed_speed_ratio) {
            moved_rigid_body.AddForce(force_vector);
        } else if (current_speed_forward > needed_speed_ratio) {
            moved_rigid_body.AddForce(-force_vector);
        }

        var relative_rotation_to_destination = 
            absolute_rotation_to_destination.to_degree() - transform.rotation.to_degree();

        turn_steering_wheels_towards_relative_angle(relative_rotation_to_destination);
        
        // moved_body.rotation_acceleration = possible_rotation;
        // moved_body.target_rotation = moving_direction_vector.to_quaternion();
        // moved_body.rotate_to_desired_direction();
    }

    public void face_rotation(Quaternion rotation) {
        
    }

    
}

}