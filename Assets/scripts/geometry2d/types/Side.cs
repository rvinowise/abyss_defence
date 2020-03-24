using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;
using rvinowise.rvi.contracts;

namespace geometry2d {

public class Side : Headspring.Enumeration<Side, int> {
    public static readonly Side LEFT = new Side(1, "LEFT");
    public static readonly Side RIGHT = new Side(-1, "RIGHT");
    public static readonly Side NONE = new Side(0, "NONE");
        
    private Side(int value, string displayName) : base(value, displayName) { }
        
    
    public static Side operator - (Side in_side) {
        return in_side.mirror();
    } 
    public Side mirror() {
        return Side.FromValue(this.Value * -1);
    }

    public static Side from_degrees(float degrees) {
        if (degrees > Mathf.Epsilon) {
            return Side.LEFT;
        } else if (degrees < Mathf.Epsilon) {
            return Side.RIGHT;
        }
        return Side.NONE;
    }

    public float turn_degrees(float degrees) {
        Contract.Assume(degrees >= 0, "not sure how to handle turning negative degrees into a direction");
        Contract.Assume(this != NONE, "not sure how to handle turning into Side.NONE");
        return degrees * this.Value;
    } 
    
    public static float operator * (Side side, float degrees) {
        return side.turn_degrees(degrees);
    } 
}
}