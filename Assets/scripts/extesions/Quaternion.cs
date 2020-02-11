using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static partial class Unity_extension
{
    /*public static Quaternion from_degrees(this Sprite sprite)
    {
        return (int)(sprite.rect.width / sprite.bounds.size.x);
    }*/
    public static Quaternion degrees_to_quaternion(float degrees) {
        return Quaternion.Euler(0f,0f,degrees);
    }
    public static float degrees(this Quaternion quaternion) {
        return quaternion.eulerAngles.z;
    }
}