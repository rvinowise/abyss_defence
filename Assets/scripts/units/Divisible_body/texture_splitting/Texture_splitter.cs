using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rvinowise.unity.extensions;

using UnityEditor;
using rvinowise.unity.geometry2d;


namespace rvinowise.unity.geometry2d {

static class Texture_splitter {
    
    //Texture2D sticking_out;

    static public Texture2D create_texture_for_polygon(
        Sprite basis,
        Polygon polygon,
        Sprite inside=null)
    {
        if (inside)
        {
            return create_texture_with_insides_for_polygon(
                basis,
                inside,
                polygon);
        }
        return create_simple_texture_for_polygon(
            basis,
            polygon);
    }

    static public Texture2D create_simple_texture_for_polygon(
        Sprite basis,
        Polygon polygon
    ) 
    {
        Vector2 adjustment = adjust_polygon_to_sprite_pivot(
            polygon, basis
        );

        Texture2D out_texture = mask_sprite_with_polygon(basis, polygon);

        polygon.move(-adjustment);

        
        return out_texture;
    }


    static public Texture2D create_texture_with_insides_for_polygon(
        Sprite basis,
        Sprite inside,
        Polygon polygon
    ) {
        Vector2 adjustment = adjust_polygon_to_sprite_pivot(
            polygon, basis
        );
        Polygon polygon_for_inside = new Polygon(polygon);
        polygon_for_inside.scale(1.3f);

        Texture2D masked_basis = mask_sprite_with_polygon(basis, polygon);
        Texture2D masked_inside = mask_sprite_with_polygon(inside, polygon_for_inside);

        polygon.move(-adjustment);

        Texture2D final_texture = Texture_drawer.instance.overlay_textures(
            masked_inside,
            masked_basis
        );
        
        return final_texture;
    }

    private static Texture2D mask_sprite_with_polygon(Sprite in_sprite, Polygon in_polygon) {
        Texture2D texture = in_sprite.texture;
        RenderTexture positioned_mask = new RenderTexture(
             texture.width, texture.height, 32, RenderTextureFormat.ARGB32);

        Texture_drawer.instance.draw_polygon_on_texture(
            positioned_mask, 
            in_sprite.pixelsPerUnit,
            in_polygon
        );
        
        Texture2D masked_texture = Texture_drawer.instance.apply_mask_to_texture(
            texture,
            positioned_mask
        );

        positioned_mask.save_to_file("positioned_mask_texture");
        positioned_mask.Release();

        //masked_texture.save_to_file("masked_texture");
        return masked_texture;
        /*return new Texture2D(
            texture.width, texture.height, TextureFormat.ARGB32, false
        ); */
    }

    

    
    private static Vector2 adjust_polygon_to_sprite_pivot(Polygon polygon, Sprite sprite) {
        Vector2 shift_in_texture = 
            sprite.pivot -
            sprite.rect.center;

        Vector2 polygon_shift = 
            shift_in_texture /
            sprite.pixelsPerUnit;
    
        polygon.move(polygon_shift);

        return polygon_shift;
    }

}

}