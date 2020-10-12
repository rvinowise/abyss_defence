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

    public static void save_to_file(this Texture2D in_texture, string filename) {
        string file_path = "./saved_textures/" + filename+".png";
        System.IO.FileInfo file = new System.IO.FileInfo(file_path);
        file.Directory.Create(); // If the directory already exists, this method does nothing.

        byte[] texture_png = in_texture.EncodeToPNG();
        System.IO.File.WriteAllBytes(file.FullName, texture_png);
    }
}