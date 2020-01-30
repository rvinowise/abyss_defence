using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static partial class Unity_extension
{
    public static Texture2D toTexture2D(this RenderTexture rTex)
    {
        Texture2D tex = new Texture2D(rTex.width, rTex.height, TextureFormat.ARGB32, false);
        RenderTexture.active = rTex;
        tex.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), 0, 0);
        RenderTexture.active = null;
        tex.Apply();
        return tex;
    }
}