using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static partial class Unity_extension
{
    public static Ray2D InverseTransformRay(this Transform transform, Ray2D ray)
    {
        return new Ray2D(
            transform.InverseTransformPoint(ray.origin),
            transform.InverseTransformDirection(ray.direction)
        );
    }

}