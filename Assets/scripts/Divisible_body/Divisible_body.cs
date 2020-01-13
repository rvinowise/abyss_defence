using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public class Divisible_body : MonoBehaviour
{
    

    public Texture2D divider_mask_l;
    public Texture2D divider_mask_r;
    public Texture2D offals;
    public Texture2D offals_divider_mask  {get;set;}

    private Shader alpha_shader {get;}
    private Material masked_material {get;set;}

    private Texture2D body;
    private Sprite body_sprite;

    //public GameObject this_preset;

/* public (Texture, Texture) divide_in_half2(Vector2 point_of_split, Vector2 direction) {
        Texture left_part = new Texture2D(body.width, body.height, TextureFormat.ARGB32, false);
        Texture right_part = new Texture2D(body.width, body.height, TextureFormat.ARGB32, false);
        return (left_part, right_part);
    }*/

    public void Awake() {
        masked_material = new Material(Shader.Find("MaskedTexture"));
        gameObject.GetComponent<Debug_monitor>().update_gizmos();

        test_create_path();
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

    public void divide_in_half(Vector2 point_of_split, Vector2 direction_of_split) {
        body_sprite = gameObject.GetComponent<SpriteRenderer>().sprite;
        body = body_sprite.texture;
       
        GameObject right_part = create_a_part(divider_mask_r, point_of_split, direction_of_split);
        GameObject left_part = create_a_part(divider_mask_l, point_of_split, direction_of_split);
    }
    
    GameObject create_a_part(
        Texture2D divider_mask_l, 
        Vector2 point_of_split,
        Vector2 direction_of_split) 
    {

        created_part.GetComponent<SpriteRenderer>().sprite = 
            create_sprite_for_part(divider_mask_l, point_of_split, direction_of_split);
        Destroy(created_part.GetComponent<PolygonCollider2D>());
        created_part.AddComponent<PolygonCollider2D>();

        created_part.GetComponent<Debug_monitor>().update_gizmos();

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
        
        retun result_sprite;
    }

/* Unity doesn't give the information about the Side of collision: have to guess it with a complex algorithm */
    /*private PolygonCollider2D create_collider_for_part(
        Vector2 point_of_split,
        Vector2 direction_of_split
    ) {
        var full_collider = gameObject.GetComponent<PolygonCollider2D>();
        var all_points = full_collider.points;
        var left_points = new List<Vector2>(all_points.size+2);
        var right_points = new List<Vector2>(all_points.size+2);
        
        //Vector2 point_of_split = collision.transform.posisiton;
        Vector2 near_split_point = gameObject.transform.InverseTransformPoint(point_of_split);
        //Vector2 direction_of_split = collision.transform.forward;
        Vector2 farther_split_point = point_of_split+direction_of_split*1.1;

        bool first_iteration = true;
        Vector2 prev_path_point;
        Tuple<Vector2, Vector2> side_of_collision;
        Tuple<Vector2, Vector2> side_oppose_collision;
        for (int i_point = 0; i_point < all_points.size; i_point++) {
            var path_point = all_points[i_point];
            var dir_to_path_point = path_point - point_of_split;
            side = Math.Sign(Angle(direction_of_split, dir_to_path_point));
  
            bool side_has_changed;
            if (first_iteration) {
                first_iteration = false;
                side_has_changed = false;
                prev_side = side;
            } else {
                side_has_changed = side != prev_side;
            }
            
            if (side_has_changed) {
                prev_side = side;
                right_points.Add(point_of_separation);
                left_points.Add(point_of_separation.copy());
            }

            if (side > 0) {
                right_points.Add(path_point);
            } else {
                left_points.Add(path_point);
            }
        }
        bool last_points_of_separation_added = (
            right_points.size + left_points.size == all_points.size + 4 
        );
        if (!last_points_of_separation_added) {
            right_points.Add(point_of_separation);
            left_points.Add(point_of_separation.copy());
        }

        return get_two_colliders_for_sprite(right_points, left_points);  
    }*/

    private Tuple<PolygonCollider2D,PolygonCollider2D> 
    get_two_colliders_for_sprite(List<Vector2> right_points, List<Vector2> left_points) {
        var left_collider = new PolygonCollider2D();
        left_collider.SetPath(0, left_points);
        var right_collider = new PolygonCollider2D();
        right_collider.SetPath(0, right_points);
        return Tuple.Create(right_collider, left_collider);
    }

    void Start()
    {
    }

    void Update()
    {

    }

     
}
