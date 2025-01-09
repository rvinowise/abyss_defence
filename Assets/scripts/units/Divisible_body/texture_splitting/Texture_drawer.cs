using System.Collections.Generic;
using UnityEngine;
using rvinowise.contracts;
using rvinowise.unity.extensions.attributes;
using rvinowise.unity.geometry2d;
using System;
using rvinowise.unity.extensions;
using UnityEngine.Experimental.Rendering;
using Random = UnityEngine.Random;

[singleton]
public class Texture_drawer: MonoBehaviour {

    public Material masked_material; //for apply_mask_to_texture
    public Material combining_material; //for overlay_textures
    public Material mask_material; //for draw_polygon_on_texture
    public Shader splitting_shader;
    
    private Material splitting_material;

    public static Texture_drawer instance {get; private set;}


    public Texture2D test_texture;
    private List<Texture2D> pooled_textures = new List<Texture2D>(100);

    void Awake() {
        Contract.Requires(instance == null, "it's a singleton");
        instance = this;
        splitting_material = new Material(splitting_shader);
    }
    void Start() {
        prepare_pool_of_textures();
    }

    
    private void prepare_pool_of_textures() {
        for (int i=0;i<pooled_textures.Capacity; i++) {
            pooled_textures.Add(
                new Texture2D(
                    test_texture.width, test_texture.height, TextureFormat.ARGB32, false
                )
            );
        }
    }
    
    public RenderTexture apply_mask_to_texture(Texture2D basis, RenderTexture mask) {
        
        /* basis.save_to_file("basis");
        mask.save_to_file("mask"); */
        RenderTexture combining_texture = 
            new RenderTexture(
                basis.width, basis.height, 0, RenderTextureFormat.Default
            );

        masked_material.SetTexture("_MainTex", basis);
        masked_material.SetTexture("_Mask", mask);
        Graphics.Blit(basis, combining_texture, masked_material);
        //combining_texture.save_to_file("combining_texture");

        //Texture2D final_texture = move_to_texture(combining_texture);

        return combining_texture;
    }

    public  void draw_polygon_on_texture(
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
        
        //mask_material.SetColor("_Color",new Color(0,1,0,1));
        mask_material.SetPass(0);
        
        Matrix4x4 m = Matrix4x4.identity;
        m = m * Matrix4x4.TRS(new Vector2(0.5f,0.5f), Quaternion.identity, Vector3.one);
        Vector2 mask_scaling = new Vector2(
            pixelsPerUnit/texture.width, pixelsPerUnit/texture.height);
        m = m * Matrix4x4.TRS(Vector3.zero, Quaternion.identity, mask_scaling);
        GL.MultMatrix(m);

        GL.Begin(GL.TRIANGLES);

        for (int i=0; i< indices.Length; i++) {
            Vector2 point = polygon.points[indices[i]];
            Color color = new Color(Random.value,Random.value,Random.value,1);
            GL.Color(color);
            GL.Vertex3(point.x, point.y, 0);
        }
        
        GL.End();
        GL.PopMatrix();
        RenderTexture.active= null;

        //texture.save_to_file("polygon");
    }

    public Texture2D overlay_textures(
        RenderTexture texture1,
        RenderTexture texture2
    ) {
        RenderTexture combining_texture = new RenderTexture(
            texture1.width, texture1.height, 0, RenderTextureFormat.ARGB32
        );

        combining_material.SetTexture("_Texture1", texture1);
        combining_material.SetTexture("_Texture2", texture2);
        Graphics.Blit(texture1, combining_texture, combining_material);
        
        return move_to_texture(combining_texture);
    }
    
    public RenderTexture draw_texture_on_texture(
        RenderTexture bottom_buffer_texture,
        RenderTexture bottom_result_texture,
        RenderTexture top_added_texture
    ) {
        // bottom_result_texture.save_to_file("bottom_result_texture before");
        // top_added_texture.save_to_file("top_added_texture");

        combining_material.SetTexture("_Bottom", bottom_result_texture);
        combining_material.SetTexture("_Top", top_added_texture);
        Graphics.Blit(top_added_texture, bottom_buffer_texture, combining_material);

        Graphics.CopyTexture(bottom_buffer_texture, bottom_result_texture);
        
        // bottom_result_texture.save_to_file("bottom_result_texture after");
        
        return bottom_result_texture;
    }
    

    public Texture2D draw_split_piece(
        Texture2D body_texture, 
        Texture2D innards_texture,
        RenderTexture body_mask,
        RenderTexture innards_mask
    ) {
        
        RenderTexture final_texture = new RenderTexture(
             body_texture.width, body_texture.height, 0, RenderTextureFormat.ARGB32
        );

        splitting_material.SetTexture("_BodyTex", body_texture);
        splitting_material.SetTexture("_InnardsTex", innards_texture);
        splitting_material.SetTexture("_BodyMask", body_mask);
        splitting_material.SetTexture("_InnardsMask", innards_mask);
        Graphics.Blit(body_texture, final_texture, splitting_material);
        
        return move_to_texture(final_texture);
    }

    private Texture2D move_to_texture(RenderTexture render_texture)
    {
        try {
            Texture2D final_texture = //pooled_textures[i_current_texture++];
                new Texture2D(
                    render_texture.width, render_texture.height, TextureFormat.ARGB32, false
                );
            Graphics.CopyTexture(render_texture, final_texture);

            RenderTexture.active = null;
            render_texture.Release();
            return final_texture;
        } catch(Exception e) {
            Debug.Log(e.ToString());
        }
        return null;
        /* RenderTexture.active = render_texture;
        final_texture.ReadPixels( 
            new Rect(0, 0, final_texture.width, final_texture.height), 0, 0
        );
        RenderTexture.active = null;
        final_texture.Apply(); */

        
    }

}