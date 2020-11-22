using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rvinowise.unity.extensions;

using rvinowise;


namespace rvinowise.unity.units.parts.transport {

public class Command_batch: parts.Command_batch {
    
    /*public float moving_direction_degrees {
        get { return _moving_direction_degrees;}
        private set { _moving_direction_degrees = value; }
    }
    private float _moving_direction_degrees;*/

    public Vector2 moving_direction_vector/* {
        get { return _moving_direction_vector; }
        private set { _moving_direction_vector = value; }
    }
    private Vector2 _moving_direction_vector*/;

    public float face_direction_degrees {
        get { return face_direction_quaternion.to_float_degrees(); }
        //private set { _face_direction_degrees = value; }
    }
    //private float _face_direction_degrees;

    public Quaternion face_direction_quaternion { get; set; }
}
}