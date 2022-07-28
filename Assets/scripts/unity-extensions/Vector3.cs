using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rvinowise.unity.extensions;

using rvinowise.unity.geometry2d.for_unmanaged;


namespace rvinowise.unity.extensions {
public static partial class Unity_extension
{
    public static Vector3 copy(this Vector3 src)
    {
        return new Vector3(src.x, src.y);
    }
    public static Vector3 rotate(this Vector3 v, float degrees)
    {
         return Quaternion.Euler(0, 0, degrees) * v;
    }
    
    public static Vector3 rotate(this Vector3 v, Quaternion rotation)
    {
        return rotation * v;
    }
    
    public static float to_dergees(this Vector3 in_direction) {
        return Mathf.Atan2(in_direction.y, in_direction.x) * Mathf.Rad2Deg;
    }
    public static Quaternion to_quaternion_OLD(this Vector3 in_direction) {
        return Quaternion.Euler(0f,0f,in_direction.to_dergees());
    }
    public static Quaternion to_quaternion(this Vector3 in_direction) {
        return Quaternion.FromToRotation(Vector3.right, in_direction);
    }
    
    public static bool within_square_from(this Vector3 position, Vector3 aim, float distance) {
        Vector3 difference = aim-position;
        if (
            (difference.x < distance)&&
            (difference.y < distance) 
        )
        {
            return true;
        }
        return false;
    }
    public static float degrees_to(this Vector3 position, Vector3 in_aim) {
        Vector3 targetDirection = in_aim - position;
        return targetDirection.to_dergees();
    }
    public static Quaternion quaternion_to(this Vector3 position, Vector3 in_aim) {
        Vector3 targetDirection = in_aim - position;
        return targetDirection.to_quaternion();
    }
    public static float sqr_distance_to(this Vector3 position, Vector3 in_aim) {
        Vector2 vector_distance = (Vector2)in_aim - (Vector2)position;
        return vector_distance.sqrMagnitude;
    }
    public static float distance_to(this Vector3 position, Vector3 in_aim) {
        Vector2 vector_distance = (Vector2)in_aim - (Vector2)position;
        return vector_distance.magnitude;
    }

    public static bool close_enough_to(this Vector3 position, Vector3 in_aim) {
        return position.sqr_distance_to(in_aim) <= 0.01f;
    }

    public static Vector3 shortened(this Vector3 vector, float length) {
        Vector3 change = vector.normalized * length;
        if (change.magnitude > vector.magnitude) {
            return Vector3.zero;
        }
        Vector3 result = vector - change;

        return result;
    }
    
    
    
}

}