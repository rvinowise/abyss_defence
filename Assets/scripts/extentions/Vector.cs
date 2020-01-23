using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using geometry.for_unmanaged;

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
}