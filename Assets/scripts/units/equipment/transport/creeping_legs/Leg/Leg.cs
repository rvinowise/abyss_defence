using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rvinowise.unity.extensions;

using rvinowise.unity.geometry2d;
using static rvinowise.unity.geometry2d.Directions;
using rvinowise.unity.units;

namespace rvinowise.unity.units.parts.limbs.creeping_legs {

[Serializable]
public partial class Leg: Limb2 
{
    /* constant characteristics */
    public Segment femur {
        get { return segment1 as creeping_legs.Segment;}
        set { segment1 = value; }
    }
    public Segment tibia {
        get { return segment2 as creeping_legs.Segment;}
        set { segment2 = value; }
    }

    public float provided_impulse = 0.2f;

    /* group of legs that being on the ground with this leg provide stability */
    public Stable_leg_group stable_group;
    
    /* where the end of the Leg should be for the maximum comfort, if the body is standing steel */
    public Vector2 optimal_position_standing {
        get {return optimal_relative_position_standing_transform.position;}
        set {optimal_relative_position_standing_transform.position = value;}
    }
    [SerializeField]
    public Transform optimal_relative_position_standing_transform;
    
    /* where the tip of it should reach, according to moving plans */
    [HideInInspector]
    public Vector2 optimal_position;
    
    // maximum distance from the optimal point after which the leg should be repositionned
    public float comfortable_distance;


    
    /* Leg itself */
    
    /* current characteristics */
    //
    [HideInInspector]
    public Vector2 holding_point;
    //whether leg is moving towards the aim or holding onto the ground
    [HideInInspector]
    public bool is_up = true;

    

    public void Awake() {
        debug = new Debug(this);
    }

    public void attach_to_body(Transform in_body) {
        parent = in_body;
    }

    public Vector2 deduce_optimal_aim() {
        float full_length = femur.tip.magnitude + tibia.tip.magnitude;
        return local_position.normalized * full_length/2;
    }

    public bool has_reached_aim() {
        /* this function is relative to the parent,
        because the desired position is relative to the parent  */
        float allowed_angle = 5f;
        if (
            (
                Quaternion.Angle(
                    femur.target_quaternion,
                    femur.transform.rotation
                ) <= allowed_angle
            )&&
            (
                Quaternion.Angle(
                    tibia.target_quaternion,
                    tibia.transform.rotation
                ) <= allowed_angle
            )
         ) 
        {
            return true;
        }
        return false;
    }
    private bool is_touching_point(Vector2 aim) {
        Vector2 tip_position = tibia.transform.TransformPoint(tibia.tip);
        if (tip_position.within_square_from(aim, 0.1f)) {
            return true;
        }
        return false;
    }

    public void raise_up() {
        is_up = true;
    }
    public void put_down() {
        is_up = false;
        holding_point = tibia.transform.TransformPoint(tibia.tip);
    }

    /* it's time to reposition */
    public bool is_twisted_uncomfortably() {
        Vector2 diff_with_optimal_point = 
            optimal_position - 
            (Vector2)tibia.transform.TransformPoint(tibia.tip);

        
        if (diff_with_optimal_point.magnitude > comfortable_distance) {
            return true;
        } 

        return false;
    }
    /* physically can't be in this position */
    public bool is_twisted_badly() {
        if (!is_within_span(femur.possible_span, femur, parent)) 
        {
            return true;
        }
        if (!is_within_span(tibia.possible_span, tibia, femur.transform))
        {
            return true;
        }
        
        return false;
    }
    private bool is_within_span(Span span, Segment segment, Transform parent) {
        float delta_degrees = parent.delta_degrees(segment.transform);
        if (
            (delta_degrees > span.max)||
            (delta_degrees < span.min)
        ) 
        {
            return false;
        }
        return true;
    }

    public void move_segments_towards_desired_direction() {
        /*femur.transform.rotation = 
            Quaternion.RotateTowards(femur.transform.rotation, 
                                     femur.target_quaternion,
                                     femur.rotation_speed * Time.deltaTime);
            
        tibia.transform.rotation = 
            Quaternion.RotateTowards(tibia.transform.rotation,
                                     tibia.target_quaternion,
                                     tibia.rotation_speed * Time.deltaTime);*/
        
        femur.rotate_to_desired_direction();
        tibia.rotate_to_desired_direction();
    }

   
    /* slower calculation but precise */
    public bool hold_onto_ground() {
        return hold_onto_point(holding_point);
    }


    

    

    public bool is_valid() {
        // this way it can be deleted from the editor when debugging
        if (!femur.game_object) {
            return false;
        }
        if (!tibia.game_object) {
            return false;
        }
        return true;
    }

    public Vector2 get_end_position_from_angles(
        Quaternion femur_rotation,
        Quaternion tibia_rotation
        ) 
    {
        Vector2 position =
            (Vector2) (femur_rotation * femur.tip) +
            (Vector2) (
                tibia_rotation *
                femur_rotation *
                tibia.tip
            );
        return position;
    }


    public void set_desired_position(Vector2 in_optimal_position) {
        optimal_position = in_optimal_position;
    }
    
    public void set_desired_directions_by_position() {
        base.set_desired_directions_by_position(optimal_position);
    }

    
} 





}
