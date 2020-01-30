using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using geometry2d;

static class Texture_drawer {

    static private Material masked_material = new Material(Shader.Find("MaskedTexture"));//{get;set;}
    static public Texture2D apply_mask_to_texture(Texture2D body, RenderTexture mask) {
        RenderTexture result_texture = new RenderTexture(
             body.width, body.height, 32, RenderTextureFormat.ARGB32
             );

        masked_material.SetTexture("_MainTex", body);
        masked_material.SetTexture("_Mask", mask);
        Graphics.Blit(body, result_texture, masked_material);
        

        RenderTexture.active = result_texture;
        Texture2D final_texture = new Texture2D(body.width, body.height, TextureFormat.ARGB32, false);
        final_texture.ReadPixels( new Rect(0, 0, final_texture.width, final_texture.height), 0, 0);
        RenderTexture.active = null;
        final_texture.Apply();
        result_texture.Release(); //a

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
        /*GL.Vertex3(0f, 0f, 0);
        GL.Vertex3(0f, 0.5f, 0);
        GL.Vertex3(0.5f, 0f, 0);*/
        
        
        
        GL.End();
        GL.PopMatrix();
        RenderTexture.active= null;
    }
}