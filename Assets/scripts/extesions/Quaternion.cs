using System.Collections;
using System.Collections.Generic;
using geometry2d;
using UnityEngine;

public static partial class Unity_extension {
    /*public static Quaternion from_degrees(this Sprite sprite)
    {
        return (int)(sprite.rect.width / sprite.bounds.size.x);
    }*/
    public static Quaternion degrees_to_quaternion(float degrees) {
        return Quaternion.Euler(0f, 0f, degrees);
    }

    public static float to_float_degrees(this Quaternion quaternion) {
        return quaternion.eulerAngles.z;
    }
    
    public static Degree to_degrees(this Quaternion quaternion) {
        return new Degree(quaternion.eulerAngles.z);
    }

    public static float degrees_to(this Quaternion from, Quaternion to) {
        return (((to.eulerAngles.z - from.eulerAngles.z) + 180f) % 360f) - 180f;
    }

    public static float abs_degrees_to(this Quaternion from, Quaternion to) {
        return Quaternion.Angle(from, to);
    }

    public static bool equal(this Quaternion from, Quaternion to) {
        return Quaternion.Angle(from, to) <= Mathf.Epsilon;
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



}