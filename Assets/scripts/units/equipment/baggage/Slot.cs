using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rvinowise;
using geometry2d;


namespace rvinowise.units.parts {

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