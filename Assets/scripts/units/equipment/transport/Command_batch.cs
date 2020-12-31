using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rvinowise.unity.extensions;

using rvinowise;


namespace rvinowise.unity.units.parts.transport {

public class Command_batch {
    

    public Vector2 moving_direction_vector;

    public float face_direction_degrees {
        get { return face_direction_quaternion.to_float_degrees(); }
    }

    public Quaternion face_direction_quaternion { get; set; }
}
}