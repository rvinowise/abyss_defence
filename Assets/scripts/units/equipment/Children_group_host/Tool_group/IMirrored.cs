using System;
using rvinowise.unity.units.parts.limbs;
using UnityEngine;

namespace rvinowise.unity.units.parts {
public interface IMirrored {
    IMirrored create_mirrored();
}

public static class Mirroring_extensions {
    public static Span mirror(this Span span) {
        return new Span(
            -span.max,
            -span.min
        );
    }

    public static Vector3 mirror(this Vector3 in_point) {
        return new Vector3(
            in_point.x,
            -in_point.y,
            in_point.z
        );
    }    
}

}