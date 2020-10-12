using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rvinowise;


namespace geometry2d {

public struct Degree {

    public float degrees;

    public static readonly Degree zero = new Degree(0f);
    
    public Degree(float in_degrees) {
        degrees = in_degrees;
    }
    
    public Quaternion to_quaternion() {
        return Quaternion.Euler(0f,0f,degrees);
    }

    public static Degree from_quaternion(Quaternion in_quaternion)
    {
        return new Degree(in_quaternion.eulerAngles.z);
    }

    public Vector2 to_vector() {
        return Quaternion.AngleAxis(degrees, Vector3.forward) * Vector3.right;
    }

    public float to_float() {
        return degrees;
    }

    public Degree normalize() {
        degrees = degrees % 360f;
        if (degrees < 0) {
            degrees = degrees + 360f;
        }
        return this;
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
    public static Degree operator * (Degree degree1, float multiplier) {
        Degree result = new Degree(degree1.degrees * multiplier);
        result.normalize();
        return result;
    }
    public static Degree operator + (Degree degree1, float degree2) {
        Degree result = new Degree(degree1.degrees + degree2);
        result.normalize();
        return result;
    }
    public static Degree operator - (Degree degree1, float degree2) {
        Degree result = new Degree(degree1.degrees - degree2);
        result.normalize();
        return result;
    }

    public Degree angle_to(Quaternion in_quaternion) {
        return angle_to(Degree.from_quaternion(in_quaternion));
    }
    public Degree angle_to(Degree target) {
        /*if (Mathf.Abs(this.degrees) == Mathf.Abs(target.degrees)) {
            return new Degree(target.degrees - this.degrees).normalize().use_minus();
        }
        return new Degree(
            this.degrees + target.degrees
        );*/
        return new Degree(target.no_minus().degrees - this.no_minus().degrees).normalize().use_minus();
    }

    public Degree use_minus() {
        if (degrees > 180) {
            degrees = degrees - 360 ; 
        } else if (degrees < -180) {
            degrees = degrees + 360 ; 
        }
        return this;
    }

    public Degree no_minus() {
        if (degrees < 0) {
            degrees = 360 + degrees; 
        }
        return this;
    }
    
    public Side side() {
        return Side.from_degrees(this.use_minus().degrees);
    }

    public Degree adjust_to_side(Side side) {
        if (side == Side.RIGHT) {
            return new Degree(-this.degrees);
        }
        return this;
    }

    public Degree change_magnitude_by_degrees(float in_degrees) {
        Degree this_degrees = this;
        float change_degrees = Mathf.Sign(this_degrees.degrees) * in_degrees;
        if (will_be_nullified(this, change_degrees)) {
            return Degree.zero;
        } else {
            return new Degree(this.degrees + change_degrees);
        }

        
        bool will_be_nullified(Degree in_current_degrees, float in_change_degrees) {
            return 
                (Mathf.Abs(this_degrees.degrees) <= Mathf.Abs(change_degrees)) &&
                (Mathf.Sign(this_degrees.degrees) != Mathf.Sign(change_degrees));
        }
    }
}
}