using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Divisible_body : MonoBehaviour
{
    public Texture2D body;
    private Sprite body_sprite;

    public Texture2D divider_mask_l;
    public Texture2D divider_mask_r;
    public Texture2D offals;
    public Texture2D offals_divider_mask  {get;set;}

    private Texture2D left_tex;
    private Texture2D right_tex;

    private Shader alpha_shader {get;}
    private Material masked_material {get;set;}

    public GameObject this_preset;

/* public (Texture, Texture) divide_in_half2(Vector2 point_of_split, Vector2 direction) {
        Texture left_part = new Texture2D(body.width, body.height, TextureFormat.ARGB32, false);
        Texture right_part = new Texture2D(body.width, body.height, TextureFormat.ARGB32, false);
        return (left_part, right_part);
    }*/

    public void Awake() {
        left_tex = new Texture2D(body.width, body.height, TextureFormat.ARGB32, false);
        right_tex = new Texture2D(body.width, body.height, TextureFormat.ARGB32, false);
        masked_material = new Material(Shader.Find("MaskedTexture"));
        body_sprite = gameObject.GetComponent<SpriteRenderer>().sprite;
        body = body_sprite.texture;
    }

    public void divide_in_half(Vector2 point_of_split, Vector2 direction_of_split) {
        
        Vector2 vector_to_split_point = (point_of_split - (Vector2)transform.position);

        Vector2 relative_point_of_split = 
            Quaternion.Inverse(transform.rotation) * vector_to_split_point;
        relative_point_of_split = Vector2.Scale(
            relative_point_of_split, new Vector2(1/body_sprite.bounds.size.x,1/body_sprite.bounds.size.y));

        Quaternion relative_split_direction = 
            Quaternion.Inverse(transform.rotation) * 
            Quaternion.FromToRotation(Vector3.right, direction_of_split);


        RenderTexture result_texture = new RenderTexture(
             body.width, body.height, 32, RenderTextureFormat.ARGB32
             );
        RenderTexture positioned_mask_texture = new RenderTexture(
             body.width, body.height, 32, RenderTextureFormat.ARGB32
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
            divider_mask_l.width / body.width * 1f, divider_mask_l.height / body.height * 1f);
        m = m * Matrix4x4.TRS(Vector3.zero, Quaternion.identity, mask_scaling);
        m = m * Matrix4x4.TRS(new Vector2(
            -(mask_scaling.x-1f) / 2f / mask_scaling.x, -(mask_scaling.y-1f)/ 2f / mask_scaling.y
            ), Quaternion.identity, Vector3.one);
       
        GL.MultMatrix(m);

        Graphics.DrawTexture(
            new Rect(0, 1, 1, -1), divider_mask_l);

        masked_material.SetTexture("_MainTex", body);
        masked_material.SetTexture("_Mask", positioned_mask_texture);
        Graphics.Blit(body, result_texture, masked_material);
        
        RenderTexture.active = result_texture;
        right_tex.ReadPixels( new Rect(0, 0, right_tex.width, right_tex.height), 0, 0);


        GL.LoadOrtho();
        GL.Clear(true, true, Color.clear);

        Graphics.DrawTexture(
            new Rect(0, 1, 1, -1), body);

        //Graphics.Blit(divider_mask_r, renderTexture);
        Graphics.DrawTexture(
            new Rect(0, 1, 1, -1), divider_mask_r);
        left_tex.ReadPixels( new Rect(0, 0, left_tex.width,left_tex.height), 0, 0);
        
        GL.PopMatrix();
        

        right_tex.Apply();
        //left_tex.Apply();

        RenderTexture.active = null;

        GameObject left_part = Instantiate(
            this_preset, 
            transform.position+(Vector3)new Vector2(0.9f,0f), 
            transform.rotation);
        GameObject right_part = Instantiate(
            this_preset, transform.position-(Vector3)new Vector2(0f,0f), transform.rotation);
        
        

        Sprite left_sprite = Sprite.Create(
            left_tex, 
            new Rect(0.0f, 0.0f, left_tex.width, left_tex.height), 
            new Vector2(0.5f, 0.5f), 100.0f);
        Sprite right_sprite = Sprite.Create(
            right_tex, 
            new Rect(0.0f, 0.0f, right_tex.width, right_tex.height), 
            new Vector2(0.5f, 0.5f), 100.0f);

        left_part.GetComponent<SpriteRenderer>().sprite = left_sprite;
        right_part.GetComponent<SpriteRenderer>().sprite = right_sprite;
        //return (left_part, right_part);
    }
    
    /*public void OnPostRender()
    {
        Debug.Log("OnPostRender");
        Graphics.DrawTexture(
            new Rect(0, 0, Camera.main.pixelWidth, Camera.main.pixelHeight), body);
    }  */

    void Start()
    {
    }

    void Update()
    {
        
    }

     
}
