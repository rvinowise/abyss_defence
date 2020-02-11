using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using geometry2d;

static class Texture_drawer {

    static private Material masked_material = 
        new Material(Shader.Find("MaskedTexture"));
    static private Material combining_material = 
        new Material(Shader.Find("Combining_textures"));
    
    static public Texture2D apply_mask_to_texture(Texture2D basis, RenderTexture mask) {
        RenderTexture combining_texture = new RenderTexture(
             basis.width, basis.height, 32, RenderTextureFormat.ARGB32
             );

        masked_material.SetTexture("_MainTex", basis);
        masked_material.SetTexture("_Mask", mask);
        Graphics.Blit(basis, combining_texture, masked_material);
    
        Texture2D final_texture = combining_texture.move_to_texture();

        return final_texture;
    }

    static public void draw_polygon_on_texture(
        RenderTexture texture, 
        float pixelsPerUnit,
        Polygon polygon) 
    {
        RenderTexture.active = texture;
        
        Triangulator tr = new Triangulator(polygon.points.ToArray());
        int[] indices = tr.Triangulate();
        
        GL.PushMatrix();
        GL.LoadOrtho();
        GL.Clear(true, true, Color.clear);
        
        Matrix4x4 m = Matrix4x4.identity;
        m = m * Matrix4x4.TRS(new Vector2(0.5f,0.5f), Quaternion.identity, Vector3.one);
        Vector2 mask_scaling = new Vector2(
            pixelsPerUnit/texture.width, pixelsPerUnit/texture.height);
        m = m * Matrix4x4.TRS(Vector3.zero, Quaternion.identity, mask_scaling);
        GL.MultMatrix(m);

        GL.Begin(GL.TRIANGLES);
        GL.Color(new Color(1f,1f,1f));
        
        for (int i=0; i< indices.Length; i++) {
            
            Vector2 point = polygon.points[indices[i]];
            GL.Vertex3(point.x, point.y, 0);
        }
        
        GL.End();
        GL.PopMatrix();
        RenderTexture.active= null;
    }

    static public Texture2D overlay_textures(
        Texture2D texture1,
        Texture2D texture2
    ) {
        RenderTexture combining_texture = new RenderTexture(
             texture1.width, texture1.height, 32, RenderTextureFormat.ARGB32
             );

        combining_material.SetTexture("_Texture1", texture1);
        combining_material.SetTexture("_Texture2", texture2);
        Graphics.Blit(texture1, combining_texture, combining_material);
        
        return combining_texture.move_to_texture();
    }
}