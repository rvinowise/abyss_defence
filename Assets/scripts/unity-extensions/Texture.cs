using UnityEngine;


namespace rvinowise.unity.extensions {

public static partial class Unity_extension
{
    public static Texture2D move_to_texture(this RenderTexture render_texture)
    {
        RenderTexture.active = render_texture;
        Texture2D final_texture = new Texture2D(
            render_texture.width, render_texture.height/* , TextureFormat.ARGB32, false */
        );
        final_texture.ReadPixels( 
            new Rect(0, 0, final_texture.width, final_texture.height), 0, 0
        );
        RenderTexture.active = null;
        final_texture.Apply();
        render_texture.Release();
        return final_texture;
    }

    /* public static Texture2D move_to_texture(this RenderTexture render_texture)
    {
        //RenderTexture.active = render_texture;
        Texture2D final_texture = new Texture2D(
            render_texture.width, render_texture.height, TextureFormat.ARGB32, false
        );
         final_texture.ReadPixels( 
            new Rect(0, 0, final_texture.width, final_texture.height), 0, 0
        );

        UnityEngine.Rendering.AsyncGPUReadbackRequest request = 
            UnityEngine.Rendering.AsyncGPUReadback.Request(render_texture, 0);
        while (!request.done)
        {
            yield return new WaitForEndOfFrame ();
        }
        byte[] rawByteArray = request.GetData<byte> ().ToArray ();

        RenderTexture.active = null;
        final_texture.Apply();
        render_texture.Release();
        return final_texture;
    } */

    public static void save_to_file(this Texture2D in_texture, string filename) {
        string file_path = string.Format(
            "./saved_textures/{0}_0.png", filename
        );
        file_path = getNextFileName(file_path);
        System.IO.FileInfo file = new System.IO.FileInfo(file_path);
        file.Directory.Create(); // If the directory already exists, this method does nothing.

        byte[] texture_png = in_texture.EncodeToPNG();
        System.IO.File.WriteAllBytes(file.FullName, texture_png);
    }

    public static void save_to_file(this RenderTexture in_render_texture, string filename) {
        Texture2D texture = in_render_texture.move_to_texture();
        texture.save_to_file(filename);
    }

    private static string getNextFileName(string fileName)
    {
        string extension = System.IO.Path.GetExtension(fileName);

        int i = 0;
        while (System.IO.File.Exists(fileName))
        {
            /* if (i == 0)
                fileName = fileName.Replace(extension, "_" + ++i + extension);
            else */
                fileName = fileName.Replace("_" + i + extension, "_" + ++i + extension);
        }

        return fileName;
    }
}

}