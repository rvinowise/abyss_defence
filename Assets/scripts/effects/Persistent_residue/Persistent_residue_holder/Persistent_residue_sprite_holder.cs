using System.Collections;
using System.Collections.Generic;
using rvinowise.unity.geometry2d;
using UnityEngine;
using rvinowise.unity.extensions;

using UnityEngine.Serialization;

namespace rvinowise.unity.effects.persistent_residue {
public class Persistent_residue_sprite_holder: 
MonoBehaviour,
IPersistent_residue_holder
{

    public int max_residue;
    public int n_frames_x = 1;

    public Sprite sprite;
    private MeshRenderer mesh_renderer;

    private Dimensionf quad_dimension;
    private int last_quad_index;

    private Mesh mesh;
    private Vector3[] vertices;
    private Vector2[] uv;
    private int[] triangles;

    private static float last_depth = 0;
    private const float depth_increment = 0.01f;
    
    private float uv_frame_step_x;


    void Awake() {
        mesh_renderer = GetComponent<MeshRenderer>();
    }
    void Start() {
        
        if (sprite != null) {
            init_for_sprite(sprite, n_frames_x);
        }

        mesh = new Mesh();

        vertices = new Vector3[4 * max_residue];
        uv = new Vector2[4 * max_residue];
        triangles = new int[6 * max_residue];

        GetComponent<MeshFilter>().mesh = mesh;

        uv_frame_step_x = 1f / n_frames_x;
    }

    public void init_for_sprite(
        Sprite sprite,
        int in_n_frames
    ) {
        
        mesh_renderer.material.mainTexture = sprite.texture;
        
        quad_dimension = new Dimensionf(
            sprite.rect.width / sprite.pixelsPerUnit / in_n_frames,
            sprite.rect.height / sprite.pixelsPerUnit
        );

        n_frames_x = in_n_frames;
    }

    /*public void add_quad(
        Vector2 in_position
    ) {
        add_quad(in_position,Quaternion.identity,1);
    }*/

    public void add_piece(
        Vector2 in_position,
        Quaternion in_rotation,
        float in_size =1,
        int in_current_frame=0
    ) {
        Vector3 relative_position = (Vector3)in_position - transform.position;
        add_quad_at_depth(
            new Vector3(
                relative_position.x,
                relative_position.y,
                last_depth
            ),
            in_rotation,
            in_size,
            in_current_frame
        );
        last_depth-=depth_increment;
    }

    public void add_quad_at_depth(
        Vector3 in_position,
        Quaternion in_rotation,
        float in_size = 1,
        int in_frame = 1
    ) {
        if (last_quad_index >= max_residue) {
            return;
        }

        int v_index = last_quad_index * 4;

        
        Vector2 top_left = new Vector2(-quad_dimension.width/2, quad_dimension.height/2);
        Vector2 top_right = new Vector2(quad_dimension.width/2, quad_dimension.height/2);
        Vector2 bottom_right = new Vector2(quad_dimension.width/2, -quad_dimension.height/2);
        Vector2 bottom_left = new Vector2(-quad_dimension.width/2, -quad_dimension.height/2);
        
        vertices[v_index] = in_position + (in_rotation*top_left * in_size);
        vertices[v_index + 1] = in_position + (in_rotation*top_right * in_size);
        vertices[v_index + 2] = in_position + (in_rotation*bottom_right * in_size);
        vertices[v_index + 3] = in_position + (in_rotation*bottom_left * in_size);



        // UV
        uv[v_index] = new Vector2(in_frame*uv_frame_step_x, 0);
        uv[v_index + 1] = new Vector2((in_frame+1)*uv_frame_step_x, 0);
        uv[v_index + 2] = new Vector2((in_frame+1)*uv_frame_step_x, 1);
        uv[v_index + 3] = new Vector2(in_frame*uv_frame_step_x, 1);

        //create triangles
        int t_index = last_quad_index * 6;

        triangles[t_index] = v_index;
        triangles[t_index + 1] = v_index + 2;
        triangles[t_index + 2] = v_index + 3;

        triangles[t_index + 3] = v_index;
        triangles[t_index + 4] = v_index + 1;
        triangles[t_index + 5] = v_index + 2;

        last_quad_index++;

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;

    }



}

}
