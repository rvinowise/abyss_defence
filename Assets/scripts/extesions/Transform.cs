using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using geometry2d;

public static partial class Unity_extension
{
    public static Ray2D InverseTransformRay(this Transform transform, Ray2D ray)
    {
        return new Ray2D(
            transform.InverseTransformPoint(ray.origin),
            transform.InverseTransformDirection(ray.direction)
        );
    }

    public static void direct_to(this Transform transform, Vector2 in_aim) {
        Vector2 targetDirection = in_aim - (Vector2)transform.position;
        transform.set_direction(targetDirection);
    }

    public static void set_direction(this Transform transform, Vector2 in_direction) {
        float needed_angle = in_direction.to_dergees();
        transform.set_direction(needed_angle);
    }
    public static void set_direction(this Transform transform, float in_direction) {
        transform.eulerAngles = Vector3.forward * in_direction;
    }

    public static float degrees_to(this Transform transform, Vector2 in_aim) {
        return ((Vector2)transform.position).degrees_to(in_aim);
    }
    public static Quaternion quaternion_to(this Transform transform, Vector2 in_aim) {
        return Quaternion.Euler(0f,0f,
            ((Vector2)transform.position).degrees_to(in_aim)
        );
    }
    public static float distance_to(this Transform transform, Vector2 in_aim) {
        return ((Vector2)transform.position).distance_to(in_aim);
    }
    public static float sqr_distance_to(this Transform transform, Vector2 in_aim) {
        return ((Vector2)transform.position).sqr_distance_to(in_aim);
    }
    public static float get_degrees(this Transform transform) {
        return ((Vector2)transform.right).to_dergees();
    }
    public static float delta_degrees(this Transform transform, Transform other) {
        return //Quaternion.Angle(transform.rotation, other.rotation);
            Mathf.DeltaAngle(transform.rotation.eulerAngles.z, other.rotation.eulerAngles.z);
    }
    public static void rotate_to(this Transform transform, float in_direction, float in_speed) {
        Quaternion needed_rotation = Quaternion.Euler(0, 0, in_direction);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, needed_rotation, in_speed);
    }


}