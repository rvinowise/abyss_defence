using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using geometry2d.for_unmanaged;

public static partial class Unity_extension
{
    public static Vector2 copy(this Vector2 src)
    {
        return new Vector2(src.x, src.y);
    }
    public static Vector2 Rotate(this Vector2 v, float degrees)
     {
         return Quaternion.Euler(0, 0, degrees) * v;
     }

    public static float to_dergees(this Vector2 in_direction) {
        return Mathf.Atan2(in_direction.y, in_direction.x) * Mathf.Rad2Deg;
    }
    public static bool within_square_from(this Vector2 position, Vector2 aim, float distance) {
        Vector2 difference = aim-position;
        if (
            (difference.x < distance)&&
            (difference.y < distance) 
        )
        {
            return true;
        }
        return false;
    }
    public static float degrees_to(this Vector2 position, Vector2 in_aim) {
        Vector2 targetDirection = in_aim - position;
        return targetDirection.to_dergees();
    }
    public static float sqr_distance_to(this Vector2 position, Vector2 in_aim) {
        Vector2 vector_distance = in_aim - (Vector2)position;
        return vector_distance.sqrMagnitude;
    }
    public static float distance_to(this Vector2 position, Vector2 in_aim) {
        Vector2 vector_distance = in_aim - (Vector2)position;
        return vector_distance.magnitude;
    }
}