using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using geometry2d;
using static geometry2d.Directions;

namespace units {
namespace limbs {

public class Leg {
    /* constant characteristics */
    public Segment femur;// = new Segment();
    public Segment tibia;// = new Segment();
    public Transform body;

    //point relative to the body where the leg is attached to it
    public Vector2 attachment;

    public Vector2 optimal_aim;
    /* current characteristics */
    //point relative to the body where the leg is trying to reach normally
    public Vector2 holding_point;
    //whether leg is moving towards the aim or holding onto the ground
    public bool is_up = true;

    public Leg(Transform in_body) {
        femur = new Segment("femur");
        tibia = new Segment("tibia");
        body = in_body;
    }
    public Vector2 deduce_optimal_aim() {
        float full_length = femur.tip.magnitude + tibia.tip.magnitude;
        return attachment.normalized * full_length/2;
    }

    public void mirror(Leg src) {
        // the base direction is to the right
        this.attachment = new Vector2(
            src.attachment.x,
            -src.attachment.y
        );
        this.femur.span = new Span(
            -src.femur.span.max,
            -src.femur.span.min
        );
        this.femur.tip = new Vector2(
            src.femur.tip.x,
            -src.femur.tip.y
        );
        this.tibia.span = new Span(
            -src.tibia.span.max,
            -src.tibia.span.min
        );
        this.tibia.tip = new Vector2(
            src.tibia.tip.x,
            -src.tibia.tip.y
        );
        this.femur.spriteRenderer.sprite = src.femur.spriteRenderer.sprite;
        this.tibia.spriteRenderer.sprite = src.tibia.spriteRenderer.sprite;
        this.femur.spriteRenderer.flipY = !src.femur.spriteRenderer.flipY;
        this.tibia.spriteRenderer.flipY = !src.tibia.spriteRenderer.flipY;
    }
    
    public void move() {
        if (is_up) {
            move_in_the_air();
        } else {
            move_on_the_ground();
        }
    }
    private void move_in_the_air() {
        if (has_reached_aim()) {
            put_down();
        } else {
            move_segments_towards_desired_direction();
            attach_to_attachment_points();
        }
    }
    private void move_on_the_ground() {
        hold_onto_ground(holding_point);
        if (is_twisted_badly()) {
            //reset_aim();
            raise_up();
        }
    }

    private bool has_reached_aim() {
        /* this function is relative to the body,
        because the desired position is relative to the body  */
        if (
            (
                body.rotation*femur.desired_relative_direction == femur.transform.rotation
            )&&
            (
                femur.rotation*tibia.desired_relative_direction == tibia.transform.rotation
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
    // store this data as Quaternions to not translate it into Eulers each step
    public void reset_aim() {
        /*float degrees_to_optimal_aim = optimal_aim.to_dergees();

        tibia.desired_relative_direction = degrees_to_quaternion(degrees_to_optimal_aim);
        femur.desired_relative_direction = degrees_to_quaternion(
            degrees_to_optimal_aim+
            (90-(optimal_aim-attachment).sqrMagnitude*700)
        );*/
        tibia.desired_relative_direction = degrees_to_quaternion(-90f);
        femur.desired_relative_direction = degrees_to_quaternion(90f);
    }
    /*private Vector2 get_optimal_aim() {

    }*/

    private void raise_up() {
        is_up = true;
    }
    private void put_down() {
        is_up = false;
        holding_point = tibia.transform.TransformPoint(tibia.tip);
    }

    public bool is_twisted_badly() {
        if (!is_whithin_span(femur, body)) 
        {
            return true;
        } else {
            if (!is_whithin_span(tibia, femur.transform))
            {
                return true;
            }
        }
        return false;
    }
    private bool is_whithin_span(Segment segment, Transform attachment) {
        //float delta_degrees = attachment.delta_degrees(segment.transform);
        float delta_degrees = attachment.delta_degrees(segment.transform);
        if (
            (delta_degrees > segment.span.max)||
            (delta_degrees < segment.span.min)
        ) 
        {
            return false;
        }
        return true;
    }

    public void move_segments_towards_desired_direction() {
        femur.transform.rotation = 
            Quaternion.RotateTowards(femur.transform.rotation, 
                                     femur.desired_relative_direction*body.transform.rotation,
                                     1f);
            
        tibia.transform.rotation = 
            Quaternion.RotateTowards(tibia.transform.rotation,
                                     tibia.desired_relative_direction*femur.transform.rotation,
                                     1f);
        
        /*Quaternion femur_relative_rotation = 
            femur.transform.rotation*Quaternion.Inverse(body.transform.rotation);
        Quaternion femur_new_relative_rotation = 
            Quaternion.RotateTowards(femur_relative_rotation, 
                                     femur.desired_relative_direction,
                                     1f);
        femur.transform.rotation = femur_new_relative_rotation*body.transform.rotation;
            
        Quaternion tibia_relative_rotation = 
            tibia.transform.rotation*Quaternion.Inverse(femur.transform.rotation);
        Quaternion tibia_new_relative_rotation = 
            Quaternion.RotateTowards(tibia_relative_rotation, 
                                     tibia.desired_relative_direction,
                                     1f);
        tibia.transform.rotation = tibia_new_relative_rotation *femur.transform.rotation;*/
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
    public void hold_onto_ground(Vector2 holding_point) {
        
        femur.position = body.TransformPoint(attachment);

        float distance_to_aim = femur.transform.distance_to(holding_point);
        float femur_angle_offset = 
            geometry2d.Triangles.get_angle_by_lengths(
                femur.length,
                distance_to_aim,
                tibia.length
            );
        femur.set_direction(femur.transform.degrees_to(holding_point)+femur_angle_offset);

        tibia.position = (Vector2)femur.transform.TransformPoint(femur.tip);

        tibia.direct_to(holding_point);
    }
    public void attach_to_attachment_points() {
        femur.position = body.TransformPoint(attachment);
        tibia.position = (Vector2)femur.transform.TransformPoint(femur.tip);
    }

    public void draw_debug_gizmos() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(holding_point, 0.01f);
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(tibia.transform.TransformPoint(tibia.tip), 0.01f);
    }
} 





}
}