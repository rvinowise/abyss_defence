using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using geometry2d;
using static geometry2d.Directions;
using rvinowise.units;

namespace rvinowise.units.parts.limbs.legs {

public partial class Leg: Limb2 
{
    /* constant characteristics */
    public Segment femur {
        get { return segment1 as legs.Segment;}
        set { segment1 = value; }
    }
    public Segment tibia {
        get { return segment2 as legs.Segment;}
        set { segment2 = value; }
    }

    public float provided_impulse = 0.2f;

    /* group of legs that being on the ground with this leg provide stability */
    public Stable_leg_group stable_group;
    
    /* where the end of the Leg should be for the maximum comfort, if the body is standing steel */
    public Vector2 optimal_relative_position_standing;
    // maximum distance from the optimal point after which the leg should be repositionned
    public float comfortable_distance;


    
    /* Leg itself */
    
    /* current characteristics */
    //
    public Vector2 holding_point;
    //whether leg is moving towards the aim or holding onto the ground
    public bool is_up = true;

    /* where the tip of it should reach, according to moving plans */
    public Vector2 optimal_position;

    public Leg(Transform inHost) {
        femur = new Segment("femur");
        tibia = new Segment("tibia");
        femur.game_object.GetComponent<SpriteRenderer>().sortingLayerName = "legs";
        tibia.game_object.GetComponent<SpriteRenderer>().sortingLayerName = "legs";
        tibia.parent = femur.transform;
        parent = inHost;
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
                    femur.desired_direction,
                    femur.transform.rotation
                ) <= allowed_angle
            )&&
            (
                Quaternion.Angle(
                    tibia.desired_direction,
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
            (Vector2)parent.transform.TransformPoint(optimal_relative_position_standing) - 
            (Vector2)tibia.transform.TransformPoint(tibia.tip);
        if (diff_with_optimal_point.magnitude > comfortable_distance) {
            return true;
        } 
        /*if (!is_within_span(femur.comfortable_span, femur, parent)) 
        {
            return true;
        } else {
            if (!is_within_span(tibia.comfortable_span, tibia, femur.transform))
            {
                return true;
            }
        }*/
        return false;
    }
    /* physically can't be in this position */
    public bool is_twisted_badly() {
        if (!is_within_span(femur.possible_span, femur, parent)) 
        {
            return true;
        } else {
            if (!is_within_span(tibia.possible_span, tibia, femur.transform))
            {
                return true;
            }
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
        femur.transform.rotation = 
            Quaternion.RotateTowards(femur.transform.rotation, 
                                     femur.desired_direction,
                                     femur.rotation_speed * Time.deltaTime);
            
        tibia.transform.rotation = 
            Quaternion.RotateTowards(tibia.transform.rotation,
                                     tibia.desired_direction,
                                     tibia.rotation_speed * Time.deltaTime);
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
            local_position +
            (Vector2) (femur_rotation * femur.tip) +
            (Vector2) (
                tibia_rotation *
                femur_rotation *
                tibia.tip
            );
        return position;
    }

    /*public void get_angles_from_end_position(
        Vector2 end_position,
        out Quaternion femur_rotation,
        out Quaternion tibia_rotation     
        ) 
    {
        
    }*/

    public void set_optimal_position(Vector2 in_optimal_position) {
        optimal_position = in_optimal_position;
        set_desired_directions_by_position(optimal_position);
    }

    
} 





}
