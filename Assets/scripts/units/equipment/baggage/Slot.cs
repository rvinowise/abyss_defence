using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rvinowise.unity.extensions;

using rvinowise;
using rvinowise.unity.geometry2d;


namespace rvinowise.unity.units.parts {

public class Slot: MonoBehaviour {

    public float depth;
    public float entering_span;

    public Orientation get_orientation_inside() {
        return new Orientation(
            new Vector2(0f, -depth),  
            new Degree(180f).to_quaternion(), 
            transform
        );
    }
}
}