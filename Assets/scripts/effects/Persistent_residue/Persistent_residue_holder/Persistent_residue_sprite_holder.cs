using System.Collections.Generic;
using rvinowise.unity.extensions;
using rvinowise.unity.extensions.pooling;
using rvinowise.unity.geometry2d;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Assertions.Comparers;


namespace rvinowise.unity {
public class Persistent_residue_sprite_holder: 
MonoBehaviour,
IPersistent_residue_holder
{

    public int n_frames_x = 1;

    public Sprite sprite;
    private MeshRenderer mesh_renderer;

    private Dimensionf quad_dimension;
    private int last_quad_index;

    private Mesh mesh;
    private Vector3[] vertices;
    private Vector2[] uv;
    private int[] triangles;
    private Color[] colors;
    
    private float uv_frame_step_x;

    /* adding each piece to the mesh is slow, we need to add many pieces at once, after they accumulate as simple game objects */
    private List<Leaving_persistent_sprite_residue> batched_residues = new List<Leaving_persistent_sprite_residue>();

    void Awake() {
        mesh_renderer = GetComponent<MeshRenderer>();

        mesh = new Mesh();
        mesh.bounds = new Bounds(Vector3.zero, new Vector3(100,100,100));
        mesh.MarkDynamic();
        GetComponent<MeshFilter>().mesh = mesh;
    }

    public void init_for_sprite(
        Sprite in_sprite,
        int in_n_frames,
        int max_residue
    ) {
        mesh_renderer.material.mainTexture = in_sprite.texture;
        
        quad_dimension = new Dimensionf(
            in_sprite.rect.width / in_sprite.pixelsPerUnit / in_n_frames,
            in_sprite.rect.height / in_sprite.pixelsPerUnit
        );

        vertices = new Vector3[4 * max_residue];
        uv = new Vector2[4 * max_residue];
        triangles = new int[6 * max_residue];
        colors = new Color[4 * max_residue];

        n_frames_x = in_n_frames;
        uv_frame_step_x = 1f / n_frames_x;
    }

    public Color next_color = Color.white;

    public Persistent_residue_sprite_holder with_color(Color in_color) {
        this.next_color = in_color;
        return this;
    }

    private const int max_batch_amount = 50;
    public void add_piece(
        Leaving_persistent_sprite_residue in_residue
    ) {
        in_residue.transform.set_z(Persistent_residue_router.instance.get_next_depth());
        
        batched_residues.Add(in_residue);
        if (batched_residues.Count > max_batch_amount) {
            add_pieces_to_mesh();
            batched_residues.Clear();
        }
    }

    private void add_pieces_to_mesh() {

        foreach (var piece in batched_residues) {
            int current_frame = 0;
            if (piece.sprite_resolver != null) {
                current_frame = piece.sprite_resolver.get_label_as_number();
            }
            
            with_color(piece.sprite_renderer.color)
            .add_piece_as_quad(
                piece.transform.position, 
                piece.transform.rotation,
                piece.sprite_renderer.transform.localScale.x,
                current_frame,
                piece.sprite_renderer.flipX,
                piece.sprite_renderer.flipY
            );
            
            piece.destroy();
        }
        update_mesh();
    }
    
    private void add_piece_as_quad(
        Vector2 in_position,
        Quaternion in_rotation,
        float in_size =1,
        int in_current_frame=0,
        bool flip_x = false,
        bool flip_y = false
    ) {
        Vector3 relative_position = (Vector3)in_position - transform.position;
        add_quad_at_depth(
            new Vector3(
                relative_position.x,
                relative_position.y,
                relative_position.z
            ),
            in_rotation,
            in_size,
            in_current_frame,
            flip_x,
            flip_y
        );
    }

    private int get_max_residue() {
        return vertices.Length / 4;
    }
    
    public void add_quad_at_depth(
        Vector3 in_position,
        Quaternion in_rotation,
        float in_size = 1,
        int in_frame = 0,
        bool flip_x = false,
        bool flip_y = false
    ) {
        if (last_quad_index >= get_max_residue()) {
            last_quad_index = 0;
        }

        int v_index = last_quad_index * 4;

        
        Vector2 top_left = new Vector2(-quad_dimension.width/2, quad_dimension.height/2);
        Vector2 top_right = new Vector2(quad_dimension.width/2, quad_dimension.height/2);
        Vector2 bottom_right = new Vector2(quad_dimension.width/2, -quad_dimension.height/2);
        Vector2 bottom_left = new Vector2(-quad_dimension.width/2, -quad_dimension.height/2);
        
        if (flip_y) {
            (top_left, bottom_left) = (bottom_left, top_left);
            (top_right, bottom_right) = (bottom_right, top_right);
        }
        if (flip_x) {
            (top_left, top_right) = (top_right, top_left);
            (bottom_left, bottom_right) = (bottom_right, bottom_left);
        }
        
        vertices[v_index] = in_position + (in_rotation*top_left * in_size);
        vertices[v_index + 1] = in_position + (in_rotation*top_right * in_size);
        vertices[v_index + 2] = in_position + (in_rotation*bottom_right * in_size);
        vertices[v_index + 3] = in_position + (in_rotation*bottom_left * in_size);

        uv[v_index] = new Vector2(in_frame*uv_frame_step_x, 1);
        uv[v_index + 1] = new Vector2((in_frame+1)*uv_frame_step_x, 1);
        uv[v_index + 2] = new Vector2((in_frame+1)*uv_frame_step_x, 0);
        uv[v_index + 3] = new Vector2(in_frame*uv_frame_step_x, 0);

        //create triangles
        int t_index = last_quad_index * 6;

        triangles[t_index] = v_index;
        triangles[t_index + 1] = v_index + 2;
        triangles[t_index + 2] = v_index + 3;

        triangles[t_index + 3] = v_index;
        triangles[t_index + 4] = v_index + 1;
        triangles[t_index + 5] = v_index + 2;

        colors[v_index] = next_color;
        colors[v_index + 1] = next_color;
        colors[v_index + 2] = next_color;
        colors[v_index + 3] = next_color;
        
        last_quad_index++;
    }

    private void update_mesh() {
        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
        mesh.colors = colors;
    }



}

}
