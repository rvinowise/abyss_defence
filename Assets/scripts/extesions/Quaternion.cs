using System.Collections;
using System.Collections.Generic;
using geometry2d;
using rvinowise.rvi;
using UnityEngine;

public static partial class Unity_extension {
    /*public static Quaternion from_degrees(this Sprite sprite)
    {
        return (int)(sprite.rect.width / sprite.bounds.size.x);
    }*/
    

    public static float to_float_degrees(this Quaternion quaternion) {
        return quaternion.eulerAngles.z;
    }
    
    public static Degree to_degree(this Quaternion quaternion) {
        return Degree.from_quaternion(quaternion);
    }
    public static Vector2 to_vector(this Quaternion quaternion) {
        return quaternion * Vector2.right;
    }
    
    public static Degree to_degrees(this Quaternion quaternion) {
        return new Degree(quaternion.eulerAngles.z);
    }
    


    public static Degree degrees_to(this Quaternion from, Quaternion to) {
        //return (((to.eulerAngles.z - from.eulerAngles.z) + 180f) % 360f) - 180f;
        return Degree.from_quaternion(from).angle_to(to).use_minus();
    }

    public static float abs_degrees_to(this Quaternion from, Quaternion to) {
        return Quaternion.Angle(from, to);
    }

    public static bool equal(this Quaternion from, Quaternion to) {
        return Quaternion.Angle(from, to) <= Mathf.Epsilon;
    }
    public static bool close_enough_to(this Quaternion from, Quaternion to) {
        return Quaternion.Angle(from, to) <= 5f;
    }

    public static Quaternion inverse(this Quaternion rotation) {
        return Quaternion.Inverse(rotation);
    }

    
    public static Side side(this Quaternion quaternion) {
        return Side.from_quaternion(quaternion);
    }

    public static Quaternion multiplied(this Quaternion quaternion, float multiplier) {
        return (quaternion.to_degrees()*multiplier).to_quaternion();
    } 

    public static Quaternion change_magnitude_by_degrees(this Quaternion quaternion, float in_degrees) {
        Degree current_degrees = new Degree(quaternion.eulerAngles.z).use_minus();
        float change_degrees = Mathf.Sign(current_degrees.degrees) * in_degrees;
        if (will_be_nullified(current_degrees, change_degrees)) {
            return Quaternion.identity;    
        } else {
            return Directions.degrees_to_quaternion(current_degrees.degrees + change_degrees);
        }

        bool will_be_nullified(Degree in_current_degrees, float in_change_degrees) {
            return 
                (Mathf.Abs(current_degrees.degrees) <= Mathf.Abs(change_degrees)) &&
                (Mathf.Sign(current_degrees.degrees) != Mathf.Sign(change_degrees));
        }
    }

}