using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using geometry2d;
//using geometry.for_unmanaged;

[RequireComponent(typeof(PolygonCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public partial class Divisible_body : MonoBehaviour
{
    public Texture2D offals;
    public Texture2D offals_divider_mask  {get;set;}

    private Shader alpha_shader {get;}
    private Material masked_material {get;set;}
    private Texture2D body;
    private Sprite body_sprite;

    public void Awake() {
        masked_material = new Material(Shader.Find("MaskedTexture"));
    }

    

    public void split_by_ray(Ray2D ray_of_split) {

        List<Polygon> collider_pieces = Polygon_splitter.split_polygon_by_ray(
            new Polygon(gameObject.GetComponent<PolygonCollider2D>().GetPath(0)),
            transform.InverseTransformRay(ray_of_split)
        );
        foreach(Polygon collider_piece in collider_pieces) {
            Texture2D texture_piece = create_texture_for_polygon(collider_piece);
            GameObject object_piece = create_gameobject_from_polygon_and_texture(
                collider_piece, texture_piece
            );
        }
        Destroy(gameObject);
    }
    Texture2D create_texture_for_polygon(
        Polygon polygon) 
    {
        body = gameObject.GetComponent<SpriteRenderer>().sprite.texture;
        RenderTexture positioned_mask_texture = new RenderTexture(
             body.width, body.height, 32, RenderTextureFormat.ARGB32
             );
        Texture_drawer.draw_polygon_on_texture(
            positioned_mask_texture, 
            gameObject.GetComponent<SpriteRenderer>().sprite.pixelsPerUnit,
            polygon);
        
        Texture2D out_texture = //gameObject.GetComponent<SpriteRenderer>().sprite.texture;
            //positioned_mask_texture.toTexture2D();
            Texture_drawer.apply_mask_to_texture(
                gameObject.GetComponent<SpriteRenderer>().sprite.texture,
                positioned_mask_texture
            );
         
        positioned_mask_texture.Release();
        return out_texture;
    }

    GameObject create_gameobject_from_polygon_and_texture(
        Polygon polygon, Texture2D texture) 
    {
        GameObject created_part = Instantiate(
            gameObject, transform.position, transform.rotation);
        
        Sprite sprite = Sprite.Create(
            texture, 
            new Rect(0.0f, 0.0f, texture.width, texture.height), 
            new Vector2(0.5f, 0.5f), 
            gameObject.GetComponent<SpriteRenderer>().sprite.pixelsPerUnit
            );

        created_part.GetComponent<PolygonCollider2D>().SetPath(0, polygon.points);
        created_part.GetComponent<SpriteRenderer>().sprite = sprite;
        return created_part;
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
        GL.PopMatrix();

        masked_material.SetTexture("_MainTex", body);
        masked_material.SetTexture("_Mask", positioned_mask_texture);
        Graphics.Blit(body, result_texture, masked_material);
        
        RenderTexture.active = result_texture;
        Texture2D final_texture = new Texture2D(body.width, body.height, TextureFormat.ARGB32, false);
        
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



    void Start()
    {
    }

    void Update()
    {

    }

     
}
