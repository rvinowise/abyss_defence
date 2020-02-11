using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using geometry2d;


namespace geometry2d {

static class Texture_splitter {
    
    //Texture2D sticking_out;


    static public Texture2D create_texture_with_insides_for_polygon(
        Sprite basis,
        Sprite inside,
        Polygon polygon
    ) 
    {
        RenderTexture positioned_mask_basis = new RenderTexture(
             basis.texture.width, basis.texture.height, 32, RenderTextureFormat.ARGB32);

        Vector2 adjustment = adjust_polygon_to_sprite_pivot(polygon, basis);

        Texture_drawer.draw_polygon_on_texture(
            positioned_mask_basis, 
            basis.pixelsPerUnit,
            polygon
        );
        
        Texture2D masked_basis = Texture_drawer.apply_mask_to_texture(
            basis.texture,
            positioned_mask_basis
        );

        RenderTexture positioned_mask_inside = new RenderTexture(
             inside.texture.width, inside.texture.height, 32, RenderTextureFormat.ARGB32);

        Polygon polygon_for_inside = new Polygon(polygon);
        polygon.move(-adjustment);
        polygon_for_inside.scale(1.3f);

        
        Texture_drawer.draw_polygon_on_texture(
            positioned_mask_inside, 
            inside.pixelsPerUnit,
            polygon_for_inside
        );

        Texture2D masked_inside = Texture_drawer.apply_mask_to_texture(
            inside.texture,
            positioned_mask_inside
        );

        Texture2D final_texture = Texture_drawer.overlay_textures(
            masked_inside,
            masked_basis
        );
        
        return final_texture;
    }

    static public Texture2D create_texture_for_polygon(
        Sprite basis,
        Polygon polygon
    ) 
    {
        Texture2D texture = basis.texture;
        RenderTexture positioned_mask_texture = new RenderTexture(
             texture.width, texture.height, 32, RenderTextureFormat.ARGB32);

        Vector2 adjustment = adjust_polygon_to_sprite_pivot(polygon, basis);

        Texture_drawer.draw_polygon_on_texture(
            positioned_mask_texture, 
            basis.pixelsPerUnit,
            polygon
        );
        
        polygon.move(-adjustment);

        Texture2D out_texture = Texture_drawer.apply_mask_to_texture(
            texture,
            positioned_mask_texture
        );
         
        positioned_mask_texture.Release();
        return out_texture;
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