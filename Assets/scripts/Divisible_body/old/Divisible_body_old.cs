using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//namespace old {
/*public partial class Divisible_body : MonoBehaviour
{

    public Texture2D offals_divider_mask  {get;set;}
    private Material masked_material {get;set;}
    private Texture2D host;
    private Sprite body_sprite;

    public void Awake() {
        masked_material = new Material(Shader.Find("MaskedTexture"));
    }

    public void test_create_path() {
        PolygonCollider2D collider = gameObject.GetComponent<PolygonCollider2D>();
        Vector2[] points =  { 
            new Vector2(0.2f,0f),
            new Vector2(0.2f,0.2f),
            new Vector2(0.25f,0.15f),
            new Vector2(0.30f,0.2f),
            new Vector2(0.30f,-0.05f),
            new Vector2(0.2f,-0.05f)
            };
        Debug.Log("paths:"+collider.pathCount);
        //collider.SetPath(collider.pathCount-1, points);
        collider.points = points;
    }


    private Sprite create_sprite_for_part(
        Texture2D divider_mask_l, 
        Vector2 point_of_split, 
        Vector2 direction_of_split) 
    {
        Vector2 vector_to_split_point = (point_of_split - (Vector2)transform.position);

        Vector2 relative_point_of_split = 
            Quaternion.Inverse(transform.rotation) * vector_to_split_point;
        relative_point_of_split = Vector2.Scale(
            relative_point_of_split, new Vector2(1/body_sprite.bounds.size.x,1/body_sprite.bounds.size.y));

        Quaternion relative_split_direction = 
            Quaternion.Inverse(transform.rotation) * 
            Quaternion.FromToRotation(Vector3.right, direction_of_split);


        RenderTexture result_texture = new RenderTexture(
             host.width, host.height, 32, RenderTextureFormat.ARGB32
             );
        RenderTexture positioned_mask_texture = new RenderTexture(
             host.width, host.height, 32, RenderTextureFormat.ARGB32
             );
        RenderTexture.active = positioned_mask_texture;

        GL.PushMatrix();
        GL.LoadOrtho();
        GL.Clear(true, true, Color.clear);

        Matrix4x4 m = Matrix4x4.identity;
        
        m = m * Matrix4x4.TRS(relative_point_of_split, Quaternion.identity, Vector3.one);
        
        m = m * Matrix4x4.TRS(new Vector2(0.5f,0.5f), Quaternion.identity, Vector3.one);
        m = m * Matrix4x4.TRS(Vector3.zero, relative_split_direction, Vector3.one);
        m = m * Matrix4x4.TRS(new Vector2(-0.5f,-0.5f), Quaternion.identity, Vector3.one);
        
        Vector2 mask_scaling = new Vector2(
            divider_mask_l.width / host.width * 1f, divider_mask_l.height / host.height * 1f);
        m = m * Matrix4x4.TRS(Vector3.zero, Quaternion.identity, mask_scaling);
        m = m * Matrix4x4.TRS(new Vector2(
            -(mask_scaling.x-1f) / 2f / mask_scaling.x, -(mask_scaling.y-1f)/ 2f / mask_scaling.y
            ), Quaternion.identity, Vector3.one);
       
        GL.MultMatrix(m);

        Graphics.DrawTexture(
            new Rect(0, 1, 1, -1), divider_mask_l);
        GL.PopMatrix();

        masked_material.SetTexture("_MainTex", host);
        masked_material.SetTexture("_Mask", positioned_mask_texture);
        Graphics.Blit(host, result_texture, masked_material);
        
        RenderTexture.active = result_texture;
        Texture2D final_texture = new Texture2D(host.width, host.height, TextureFormat.ARGB32, false);
        
        final_texture.ReadPixels( new Rect(0, 0, final_texture.width, final_texture.height), 0, 0);
        RenderTexture.active = null;
        
        final_texture.Apply();

        GameObject created_part = Instantiate(
            gameObject, transform.position-(Vector3)new Vector2(0f,0f), transform.rotation);
        
        Sprite result_sprite = Sprite.Create(
            final_texture, 
            new Rect(0.0f, 0.0f, final_texture.width, final_texture.height), 
            new Vector2(0.5f, 0.5f), 100.0f);
        
        return result_sprite;
    }
}

//}*/