using System.Collections.Generic;
using UnityEngine;


namespace rvinowise.unity.extensions {
public static partial class Unity_extension
{
    public static Vector2 copy(this Vector2 src)
    {
        return new Vector2(src.x, src.y);
    }
    public static Vector2 rotate(this Vector2 v, float degrees)
    {
         return Quaternion.Euler(0, 0, degrees) * v;
    }
    
    public static Vector3 rotate(this Vector2 v, Quaternion rotation)
    {
        return rotation * v;
    }
    
    public static float to_dergees(this Vector2 in_direction) {
        return Mathf.Atan2(in_direction.y, in_direction.x) * Mathf.Rad2Deg;
    }
    public static Quaternion to_quaternion_OLD(this Vector2 in_direction) {
        return Quaternion.Euler(0f,0f,in_direction.to_dergees());
    }
    public static Quaternion to_quaternion(this Vector2 in_direction) {
        return Quaternion.FromToRotation(Vector2.right, in_direction);
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
    public static Quaternion quaternion_to(this Vector2 position, Vector2 in_aim) {
        Vector2 targetDirection = in_aim - position;
        return targetDirection.to_quaternion();
    }
    public static float sqr_distance_to(this Vector2 position, Vector2 in_aim) {
        Vector2 vector_distance = in_aim - (Vector2)position;
        return vector_distance.sqrMagnitude;
    }
    public static float distance_to(this Vector2 position, Vector2 in_aim) {
        Vector2 vector_distance = in_aim - (Vector2)position;
        return vector_distance.magnitude;
    }

    public static bool close_enough_to(this Vector2 position, Vector2 in_aim) {
        return position.sqr_distance_to(in_aim) <= 0.01f;
    }
   
    public static List<Vector3> to_list_vector3 (this List<Vector2> list2)
    {
        return new List<Vector3>(
            System.Array.ConvertAll (list2.ToArray(), getV3fromV2)
        );
    }
     
    private static Vector3 getV3fromV2 (Vector2 vector2)
    {
        return new Vector3(vector2.x, vector2.y);
    }

    /*public static Vector2(this Vector2 in_vector) {
        return new Vector2(
            in_vector.x + 
        );
    }*/

    public static Vector2 shortened(this Vector2 vector, float length) {
        Vector2 change = vector.normalized * length;
        if (change.magnitude > vector.magnitude) {
            return Vector2.zero;
        }
        Vector2 result = vector - change;

        return result;
    }
    
    public static bool is_normalized(
        this Vector2 vector
    ) {
        return Mathf.Abs(vector.magnitude-1) < 0.1f;
    } 
    
    
}

}