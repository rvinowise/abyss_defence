using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class Unity_extension
{
    public static Vector2 copy(this Vector2 src)
    {
        return new Vector2(src.x, src.y);
    }


}