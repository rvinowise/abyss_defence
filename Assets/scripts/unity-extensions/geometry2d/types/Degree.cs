using System;
using UnityEngine;


namespace rvinowise.unity.geometry2d {

[Serializable]
public struct Degree {

    public float degrees;

    public static readonly Degree zero = new Degree(0f);
    
    public Degree(float in_degrees) {
        degrees = in_degrees;
    }
    public Degree(Quaternion in_quaternion) {
        degrees = in_quaternion.eulerAngles.z;
    }
    
    public static implicit operator Degree(float in_degrees)
    {
        return new Degree(in_degrees);
    }
    public static implicit operator float(Degree in_degree)
    {
        return in_degree.degrees;
    }

    public static implicit operator Degree(Quaternion in_quaternion)
    {
        return new Degree(in_quaternion.eulerAngles.z);
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

    public Degree normalized() {
        float new_degrees = degrees % 360f;
        if (new_degrees < 0) {
            new_degrees += 360f;
        }
        return new Degree(new_degrees);
    }

    public static Degree operator + (Degree degree1, Degree degree2) {
        return new Degree(degree1.degrees + degree2.degrees);
    }
    public static Degree operator - (Degree degree1, Degree degree2) {
        return new Degree(degree1.degrees - degree2.degrees);
    }
    public static Degree operator * (Degree degree1, float multiplier) {
        return new Degree(degree1.degrees * multiplier);
    }
    public static Degree operator + (Degree degree1, float degree2) {
        return new Degree(degree1.degrees + degree2);
    }
    public static Degree operator - (Degree degree1, float degree2) {
        return new Degree(degree1.degrees - degree2);
    }

    public Degree angle_to(Quaternion in_quaternion) {
        return angle_to(Degree.from_quaternion(in_quaternion));
    }
    public Degree angle_to(Degree target) {
        return new Degree(target.no_minus() - this.no_minus()).normalized().use_minus();
    }

    /* public Degree angle_to(float target) {
        return angle_to(new Degree(target));
    } */

    
    public static Degree operator * (Quaternion quaternion, Degree degree) {
        return new Degree(quaternion * degree.to_quaternion());
    }

    public Degree use_minus() {
        if (degrees >= 180) {
            return new Degree(degrees - 360); 
        } else if (degrees < -180) {
            return new Degree(degrees + 360); 
        }
        return this;
    }

    public Degree no_minus() {
        if (degrees < 0) {
            return new Degree(360 + degrees); 
        }
        return this;
    }
    
    public Side side() {
        return Side.from_degrees(this.use_minus());
    }

    public Degree adjust_to_side(Side side) {
        if (side == Side.RIGHT) {
            return new Degree(-this.degrees);
        }
        return this;
    }

    public Degree change_magnitude_by_degrees(float in_degrees) {
        Degree this_degrees = this;
        float change_degrees = Mathf.Sign(this_degrees) * in_degrees;
        if (will_be_nullified(this, change_degrees)) {
            return Degree.zero;
        } else {
            return new Degree(this.degrees + change_degrees);
        }

        
        bool will_be_nullified(Degree in_current_degrees, float in_change_degrees) {
            return 
                (Mathf.Abs(this_degrees) <= Mathf.Abs(change_degrees)) &&
                (Mathf.Sign(this_degrees) != Mathf.Sign(change_degrees));
        }
    }
}
}