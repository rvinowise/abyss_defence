using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rvinowise.unity.extensions;

using rvinowise;
using rvinowise.unity.geometry2d;
using Point = UnityEngine.Vector2;
using Random = UnityEngine.Random;

namespace rvinowise.unity.effects.trails.mesh_impl {

public class Segment {
    //public List<Point> points = new List<Point>(2);
    public Point[] points = new Point[2];
    
    public Point left_point {
        get { return points[0]; }
        set{ points[0] = value; }
    }
    public Point right_point {
        get { return points[1]; }
        set{ points[1] = value; }
    }
    
    public Point position {get;private set;}
    
    /* parameters */
    public static float width_variation = 0.04f;
    public float width = default_width;// + Random.Range(-width_variation, width_variation);
    public float width_change = 0.01f;

    private const float default_width = 5f;
    
    public Point moving_vector;
    public bool is_abruption;

    public Segment(
        Point in_position, 
        Point in_direction,
        Point in_moving_vector,
        float in_width = default_width
    ) {
        position = in_position;
        moving_vector = in_moving_vector;
        width = in_width;
        points[0] = (
            in_position + (in_direction * width/2).rotate(90f)
        );
        points[1] = (
            in_position + (in_direction * width/2).rotate(-90f)
        );
    }

    public static Segment new_abruption(
        Point in_position, 
        Point in_direction,
        float in_width = default_width
    ) {
        Segment segment = new Segment(
            in_position, in_direction, in_width
        );
        segment.is_abruption = true;
        return segment;
    }
    public static Segment new_continuation(
        Point in_position, 
        Point in_direction,
        float in_width = default_width
    ) {
        Segment segment = new Segment(
            in_position, in_direction, in_width
        );
        segment.is_abruption = false;
        return segment;
    }
    public Segment(
        Point in_position, 
        Point in_direction,
        float in_width = default_width
    ) {
        width = in_width;
        position = in_position;
        points[0] = (
            in_position + (in_direction * width/2).rotate(90f)
        );
        points[1] = (
            in_position + (in_direction * width/2).rotate(-90f)
        );
    }

    public void move() {
        left_point = left_point + 
                     (moving_vector)
                     *Time.deltaTime;
        right_point = right_point + 
                      (moving_vector)
                      *Time.deltaTime;
    }

    public void move_and_rotate(
        Point position,
        Point direction
    ) {
        this.position = position;
        Point left_point_offset = (Vector2)(Directions.degrees_to_quaternion(90f) * direction) *
            width/2;
       
        left_point = 
            (Vector2)position + left_point_offset;
        right_point = 
            (Vector2)position - left_point_offset;
        
    }
}
}