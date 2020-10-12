using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static partial class Unity_extension
{
    /*public static int pixelsPerUnit(this Sprite sprite)
    {
        return (int)(sprite.rect.width / sprite.bounds.size.x);
    }*/

    public static Vector2 to_texture_coordinates(this Sprite sprite, Vector2 point) {
        return 
            point / 
            new Vector2(
                sprite.rect.width,
                sprite.rect.height
            );
    }
    
    
}