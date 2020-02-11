using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static partial class Unity_extension
{
    public static Texture2D move_to_texture(this RenderTexture render_texture)
    {
        RenderTexture.active = render_texture;
        Texture2D final_texture = new Texture2D(
            render_texture.width, render_texture.height, TextureFormat.ARGB32, false
        );
        final_texture.ReadPixels( 
            new Rect(0, 0, final_texture.width, final_texture.height), 0, 0
        );
        RenderTexture.active = null;
        final_texture.Apply();
        render_texture.Release();
        return final_texture;
    }
}