using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace geometry2d {

public static class Directions {
    public static Quaternion degrees_to_quaternion(float degrees) {
        return Quaternion.Euler(0f,0f,degrees);
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
}

}