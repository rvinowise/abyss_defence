using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rvinowise;


namespace geometry2d {

public struct Degree {

    public float degrees;
    
    public Degree(float in_degrees) {
        degrees = in_degrees;
    }
    
    public Quaternion to_quaternion() {
        return Quaternion.Euler(0f,0f,degrees);
    }

    public Vector2 to_vector() {
        return Quaternion.AngleAxis(degrees, Vector3.forward) * Vector3.right;
    }

    public float to_float() {
        return degrees;
    }

    public void normalize() {
        degrees = degrees % 360f;
        if (degrees < 0) {
            degrees = degrees + 360f;
        }
        
    }

    public static Degree operator + (Degree degree1, Degree degree2) {
        Degree result = new Degree(degree1.degrees + degree2.degrees);
        result.normalize();
        return result;
    }
    public static Degree operator - (Degree degree1, Degree degree2) {
        Degree result = new Degree(degree1.degrees - degree2.degrees);
        result.normalize();
        return result;
    }
}
}