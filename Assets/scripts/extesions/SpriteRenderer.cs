using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static partial class Unity_extension {
    
    
    public static Vector2 get_units_size(this SpriteRenderer sprite_renderer) {
        return new Vector2(
            sprite_renderer.sprite.rect.width/sprite_renderer.sprite.pixelsPerUnit, 
            sprite_renderer.sprite.rect.height/sprite_renderer.sprite.pixelsPerUnit
        );
    }
}
