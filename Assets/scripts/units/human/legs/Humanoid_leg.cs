using System;
using rvinowise.unity.extensions;
using rvinowise.unity.actions;
using UnityEngine;
using Action = rvinowise.unity.actions.Action;

namespace rvinowise.unity {


public class Humanoid_leg: 
    MonoBehaviour
    ,ILeg
{
    #region ILeg
    
    public float provided_impulse = 0.2f;
    public float get_provided_impulse() => provided_impulse;
    
    /* where the end of the Leg should be for the maximum comfort, if the body is standing steel */
    public Transform optimal_relative_position_standing_transform;
    public Vector2 get_optimal_position_standing() => optimal_relative_position_standing_transform.position;

    public Transform attachment_point_transform;
    public Vector2 get_attachment_point() => attachment_point_transform.position;
    
    public Stable_leg_group stable_group { get; set; }

    
    /* where the tip of it should reach, according to moving plans */
    private Vector2 optimal_position;
    
    // maximum distance from the optimal point after which the leg should be repositioned
    public float reposition_distance = 0.2f;
    public float maximum_length = 0.5f;

    
    public float moving_offset_distance = 0.3f;
    public float get_moving_offset_distance() => moving_offset_distance;

    public Turning_element turning_element;
    public Turning_element movable_body;
    
    public Vector2 holding_point{get;protected set;}
    
    
    public bool up = true;
    public bool is_up() => up;


    public bool is_twisted_uncomfortably() {
        Vector2 diff_with_optimal_point = 
            optimal_position - (Vector2)transform.position;
        
        if (diff_with_optimal_point.magnitude > reposition_distance) {
            return true;
        } 

        return false;
    }
    public virtual void raise_up() {
        up = true;
    }
    public void put_down() {
        up = false;
        holding_point = transform.position;
    }
    public bool hold_onto_ground() {
        return true;
    }

    
    public virtual void set_desired_position(Vector2 in_optimal_position) {
        optimal_position = in_optimal_position;
    }

    public float maximum_rotation_difference = 60;
    public bool is_twisted_badly() {
        return
            transform.position.distance_to(get_attachment_point()) > maximum_length
            ||
            transform.rotation.abs_degrees_to(movable_body.rotation) > maximum_rotation_difference;
    }

    public void move_segments_towards_desired_direction() {
        var moving_direction = (optimal_position - (Vector2) transform.position).normalized;
        Vector2 delta_movement = Time.deltaTime * movement_speed * moving_direction;
        transform.position += (Vector3)delta_movement;
        turning_element.target_rotation = movable_body.rotation;
        turning_element.rotate_to_desired_direction();
    }

    public bool has_reached_aim() {
        return transform.position.distance_to(optimal_position) < 0.01;
    }
    
    #endregion ILeg
    
    #region Humanoid_leg itself

    public float movement_speed = 0.3f;
    
    #endregion
    
    #region IActor

    private Action_runner action_runner;
    public Action current_action { get; set; }

    public void on_lacking_action() {
        Idle.create(this).start_as_root(action_runner);
    }

    public void init_for_runner(Action_runner in_action_runner) {
        this.action_runner = in_action_runner;
    }

    #endregion IActor
    
    #region debug
    public void draw_positions() {
        const float sphere_size = 0.1f;
        Gizmos.color = new Color(1f,1f,0f,0.4f);
        Gizmos.DrawSphere(holding_point, sphere_size);
        
        Gizmos.color = new Color(1f,1f,1f,0.4f);
        Gizmos.DrawSphere(
            get_optimal_position_standing(), sphere_size);
        
        Gizmos.color = new Color(0f,0f,1f,0.4f);
        Gizmos.DrawSphere(
            optimal_position, sphere_size);
        
    }
    public void draw_desired_directions() {
        
    }

    public void draw_directions(Color in_color, float in_time) {
        
    }
    
    #endregion

    

    
}

}