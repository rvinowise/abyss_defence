using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using geometry2d;
using static geometry2d.Directions;
using rvinowise.units;

namespace rvinowise.units.parts.limbs.legs {

public class Leg: Limb2 
{
    /* constant characteristics */
    public Segment femur {
        get { return segment1 as legs.Segment;}
        set { segment1 = value; }
    }
    public Segment tibia {
        get { return segment1 as legs.Segment;}
        set { segment1 = value; }
    }

    public int folding_direction { get; set; } //1 of -1
    public float provided_impulse = 0.20f;

    /* group of legs that being on the ground with this leg provide stability */
    public Stable_leg_group stable_group;
    
    /* where the end of the Leg should be for the maximum comfort, if the body is standing steel */
    public Vector2 optimal_relative_position_standing;
    // maximum distance from the optimal point after which the leg should be repositionned
    public float comfortable_distance;

    /* Child interface */
    public override Transform host {
        get { return femur.host; }
        set { femur.host = value; }
    }

    public override Vector2 attachment {
        get {
            return femur.attachment;
        }
        set {
            femur.attachment = value;
        }
    }

    
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
        tibia.host = femur.transform;
        host = inHost;
        debug = new Debug(this);
    }

    public void attach_to_body(Transform in_body) {
        host = in_body;
    }

    public Vector2 deduce_optimal_aim() {
        float full_length = femur.tip.magnitude + tibia.tip.magnitude;
        return attachment.normalized * full_length/2;
    }

    public bool has_reached_aim() {
        /* this function is relative to the host,
        because the desired position is relative to the host  */
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
            (Vector2)host.transform.TransformPoint(optimal_relative_position_standing) - 
            (Vector2)tibia.transform.TransformPoint(tibia.tip);
        if (diff_with_optimal_point.magnitude > comfortable_distance) {
            return true;
        } 
        /*if (!is_within_span(femur.comfortable_span, femur, host)) 
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
        if (!is_within_span(femur.possible_span, femur, host)) 
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
    private bool is_within_span(Span span, Segment segment, Transform host) {
        float delta_degrees = host.delta_degrees(segment.transform);
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

    /* faster calculation but not precise */
    public void hold_onto_ground_FAST(Vector2 holding_point) {
        tibia.direct_to(holding_point);
        femur.set_direction(
            femur.transform.degrees_to(holding_point)+
            (90f-femur.transform.sqr_distance_to(holding_point)*1440f)
        );
    }
    /* slower calculation but precise */
    public bool hold_onto_ground() {
        
        femur.attach_to_host();

        float distance_to_aim = femur.transform.distance_to(holding_point);
        float femur_angle_offset = 
            geometry2d.Triangles.get_angle_by_lengths(
                femur.length,
                distance_to_aim,
                tibia.length
            );
        if (float.IsNaN(femur_angle_offset)) {
            return false;    
        }
        femur.set_direction(
            femur.transform.degrees_to(holding_point)+
            (femur_angle_offset*(float)folding_direction)
        );

        tibia.attach_to_host();

        tibia.direct_to(holding_point);
        return true;
    }

    public void attach_to_attachment_points() {
        femur.attach_to_host();
        tibia.attach_to_host();
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
            attachment +
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

    public new class Debug: Limb2.Debug {
        private readonly Leg leg;

        protected override Limb2 limb2 {
            get { return leg; }
        }

        public Debug(Leg parent):base(parent) {
            leg = parent;
        }

        public void draw_positions() {
            if (debug_off) {
                return;
            }
            /*if (name != "left_hind_leg") {
                return;
            }*/
            /*Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(
                leg.segment2.transform.TransformPoint(leg.segment2.tip), sphere_size);*/
            
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(leg.holding_point, sphere_size);
            
            Gizmos.color = Color.white;
            Gizmos.DrawSphere(
                leg.host.TransformPoint(leg.optimal_relative_position_standing), sphere_size);
            
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(
                leg.optimal_position, sphere_size);
           
        }
    }
    public Leg.Debug debug {
        get { return _debug; }
        private set { _debug = value; }
    }
    private Leg.Debug _debug;

    protected override Limb2.Debug debug_limb {
        get { return _debug; }
    }
} 





}
