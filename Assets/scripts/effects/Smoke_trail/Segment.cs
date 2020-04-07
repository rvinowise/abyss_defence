using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rvinowise;
using geometry2d;
using Point = UnityEngine.Vector2;
using Random = UnityEngine.Random;

namespace rvinowise.effects.trails {

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
    
    public Point position {
        get { return left_point; }
        set{ left_point = value; }
    }
    
    /* parameters */
    public static float width_variation = 0.04f;
    public float width = 0.07f + Random.Range(-width_variation, width_variation);
    public float width_change = 0.01f;

    public Point moving_vector;

    public Segment(
        Point in_position, 
        Point in_direction,
        Point in_moving_vector
    ) {
        /*points.Add(in_position);
        points.Add(
            in_position + (in_direction * width).rotate(-90f)
        );*/
        
        points[0] = (
            in_position + (in_direction * width/2).rotate(90f)
        );
        points[1] = (
            in_position + (in_direction * width/2).rotate(-90f)
        );
        
        moving_vector = in_moving_vector;
    }

    public void move() {
        left_point = left_point + 
                     (moving_vector)
                     *Time.deltaTime;
        right_point = right_point + 
                      (moving_vector)
                      *Time.deltaTime;
    }
}
}