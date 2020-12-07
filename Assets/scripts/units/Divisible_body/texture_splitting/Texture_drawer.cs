using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rvinowise.unity.extensions;
using rvinowise.contracts;
using rvinowise.unity.extensions.attributes;
using rvinowise.unity.geometry2d;

[singleton]
public class Texture_drawer: MonoBehaviour {

    [SerializeField]
    private Material masked_material;
    //    Resources.Load<Material>("shaders/masked_texture");

    [SerializeField]
    private Material combining_material;
    //    Resources.Load<Material>("shaders/combining_textures");

    [SerializeField]
    private Material mask_material;
    //    Resources.Load<Material>("shaders/default");

    public static Texture_drawer instance {get; private set;}


    public Texture2D test_texture;
    private List<Texture2D> pooled_textures = new List<Texture2D>(100);
    private int i_current_texture = 0;

    void Awake() {
        Contract.Requires(instance == null, "it's a singleton");
        instance = this;
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
    
    public  Texture2D apply_mask_to_texture(Texture2D basis, RenderTexture mask) {
        
        /* basis.save_to_file("basis");
        mask.save_to_file("mask"); */
        RenderTexture combining_texture = new RenderTexture(
             basis.width, basis.height, 0, RenderTextureFormat.Default
        );

        masked_material.SetTexture("_MainTex", basis);
        masked_material.SetTexture("_Mask", mask);
        Graphics.Blit(basis, combining_texture, masked_material);
        //combining_texture.save_to_file("combining_texture");

        Texture2D final_texture = move_to_texture(combining_texture);

        return final_texture;
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
            GL.Vertex3(point.x, point.y, 0);
        }
        
        GL.End();
        GL.PopMatrix();
        RenderTexture.active= null;

        //texture.save_to_file("polygon");
    }

    public  Texture2D overlay_textures(
        Texture2D texture1,
        Texture2D texture2
    ) {
        RenderTexture combining_texture = new RenderTexture(
             texture1.width, texture1.height, 0, RenderTextureFormat.ARGB32
             );

        combining_material.SetTexture("_Texture1", texture1);
        combining_material.SetTexture("_Texture2", texture2);
        Graphics.Blit(texture1, combining_texture, combining_material);
        
        return move_to_texture(combining_texture);
    }
    
    private Texture2D move_to_texture(RenderTexture render_texture)
    {
        Texture2D final_texture = pooled_textures[i_current_texture++];
        /* RenderTexture.active = render_texture;
        final_texture.ReadPixels( 
            new Rect(0, 0, final_texture.width, final_texture.height), 0, 0
        );
        RenderTexture.active = null;
        final_texture.Apply(); */

        Graphics.CopyTexture(render_texture, final_texture);

        RenderTexture.active = null;
        render_texture.Release();
        return final_texture;
    }

}