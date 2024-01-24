using UnityEngine;
using Vector2 = System.Numerics.Vector2;

namespace rvinowise {
public static class Vector {
    public static Vector2 create_random(float min, float max) {
        return new Vector2(
            min + Random.value * max,
            min + Random.value * max
        );
    }
}
}