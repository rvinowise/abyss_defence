using UnityEngine;


namespace rvinowise.unity.geometry2d {

public static class Directions {
    public static Quaternion degrees_to_quaternion(float degrees) {
        return Quaternion.Euler(0f,0f,degrees);
    }

    public static float normalize_degrees(float degrees) {
        float norlmalized = degrees % 360f;
        if (degrees < 0) {
            norlmalized = degrees + 360f;
        }
        return norlmalized;
    }

    public static Vector2 degrees_to_vector(float degrees) {
        return Quaternion.AngleAxis(degrees, Vector3.forward) * Vector3.right;
    }


}


public static class Triangles {
    public static float get_angle_by_lengths(float side1, float side2, float opposite_side) {
        float cos_of_angle = 
            (Mathf.Pow(side1,2) + Mathf.Pow(side2,2) - Mathf.Pow(opposite_side,2))
            /
            (2*side1*side2);
        return Mathf.Acos(cos_of_angle)*Mathf.Rad2Deg;
    }

    public static Quaternion get_quaternion_by_lengths(float side1, float side2, float opposite_side) {
        float degrees = get_angle_by_lengths(side1, side2, opposite_side);
        return new Degree(degrees).to_quaternion();
    }
}

}