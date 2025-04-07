using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using rvinowise.unity.extensions;
using rvinowise.contracts;
using Point = UnityEngine.Vector2;
using Quaternion = UnityEngine.Quaternion;
using Random = UnityEngine.Random;

namespace rvinowise.unity.effects.trails.mesh_impl {

public class Smoke_trail_emitter_static_tip:
MonoBehaviour
{
    
    /* parameters */
    public float distance_between_segments = 1f;
    public float width = 0.2f;
    public float fade_speed = 1f;
    public float texture_length = 4f;
    public float decorative_update_frequency = 0.1f;
    
    public UnityEngine.Events.UnityEvent on_disappeared;
    public GameObject mesh_object;/* needed because the mesh uses global coordinates */

    private float alpha;
    
    private MeshFilter mesh_filter;
    private MeshRenderer mesh_renderer;
    
    /* fields for inner functionality */
    private readonly List<Segment> segments = new List<Segment>();


    private Segment last_segment {get{
        return segments.Last();
    }}
    private readonly List<Vector3> vertices = new List<Vector3>();
    private readonly List<int> triangles = new List<int>();
    private readonly List<Vector2> main_uvs = new List<Vector2>();
    private readonly List<Vector2> noise_uvs = new List<Vector2>();
    private Mesh mesh;
    
    
    protected void Awake() {
        init_components();
        
        mesh_object.transform.SetParent(null);
        mesh_object.transform.position =
            new Vector3(0,0,transform.position.z);
        mesh_object.transform.rotation = Quaternion.identity;
        
        recreate_trail_from_scratch();
    }

    public void recreate_trail_from_scratch() {
        this.enabled = true;
        
        alpha = 1;
        apply_alpha(alpha);
        mesh_renderer.material.SetFloat(shader_start_time, Time.time);
        mesh_object.SetActive(true);
        mesh.Clear();
        vertices.Clear();
        triangles.Clear();
        main_uvs.Clear();
        segments.Clear();

        init_first_segment(transform.position,transform.rotation.to_vector());
        
        last_decorative_update = Time.time;
        InvokeRepeating(nameof(decorative_update), decorative_update_frequency, decorative_update_frequency);
    }

    private void init_components() {
        mesh_filter = mesh_object.GetComponent<MeshFilter>();
        mesh_renderer = mesh_object.GetComponent<MeshRenderer>();
        mesh = mesh_filter.mesh;
        mesh.MarkDynamic();
    }

    
    public void init_first_segment(
        Vector2 start_position,
        Vector2 in_direction
    ) {
        //Debug.Log($"TRAIL: [{name}]init_first_points start_position={start_position} direction={in_direction.to_dergees()}");
        add_segment(
            start_position,
            in_direction
        );
    }

    public void add_bending_at(
        Vector2 position,
        Vector2 new_direction
    )
    {
        //Debug.Log($"TRAIL: [{name}]add_bending_at({position}, {new_direction})");
        Contract.Requires(new_direction.is_normalized(), "direction should be a normalized vector");
        build_trail_necessarily_reaching_point(position);
        segments.Last().is_abruption = true;
        
        add_new_segment_turned_to_new_direction(position, new_direction);
        add_last_segment_sticking_to_emitter(position);

        add_new_segments_to_mesh();

        void add_new_segment_turned_to_new_direction(Vector2 _position, Vector2 _new_direction) {
            segments.Add(
                Segment.new_continuation(
                    _position,
                    _new_direction,
                    width
                )
            );
        }
    }

    public void visit_final_point(Vector2 position) {
        //Debug.Log($"TRAIL: [{name}]visit_final_point({position})");
        bool trail_grew = build_trail_necessarily_reaching_point(position);
        if (trail_grew) {
            add_new_segments_to_mesh();
        }
    }

    protected void Update() {
        bool trail_grew = built_trail_compensating_flight_of_emitter();
        if (trail_grew) {
            add_new_segments_to_mesh();
        }
    }


    public bool has_visible_parts()
    {
        return alpha > 0;
    }

    
    private bool build_trail_necessarily_reaching_point(
        Point to_point
    ) {
        if (
            last_segment.close_enough_to(to_point)
        ) {
            //Debug.Log($"TRAIL: [{name}]build_trail_necessarily_reaching_point last_segment[{last_segment.position}] is close enough to point[{to_point}]");
            return false;
        }
        //Debug.Log($"TRAIL: [{name}]build_trail_necessarily_reaching_point({to_point})");

        int segments_needed = new_segments_needed(
            to_point,
            out var direction_to_emitter
        );
        
        add_segments_in_direction(
            segments_needed,
            direction_to_emitter
        );

        if (
            !last_segment.close_enough_to(to_point)
        ) {
            add_segment(
                to_point,
                direction_to_emitter
            );
        }

        return true;
    }
    private bool built_trail_compensating_flight_of_emitter() {
        int segments_needed = new_segments_needed(
            transform.position,
            out var direction_to_goal
        );
        if (segments_needed == 0) {
            return false;
        }
        //Debug.Log($"TRAIL: [{name}]built_trail_compensating_flight_of_emitter segments_needed={segments_needed}");
        
        add_segments_in_direction(
            segments_needed,
            direction_to_goal
        );

        add_last_segment_sticking_to_emitter(transform.position);
        return true;
    }


    private void add_segments_in_direction(
        int segments_n,
        Point direction_vector
    ) {
        if (segments_n == 0) {
            return;
        }
        Point step_to_next_segment = direction_vector * 
                                     distance_between_segments;

        for (int i=0; i<segments_n; i++) {
            add_segment(
                next_position(),
                direction_vector
            );
        }

        Vector2 next_position() {
            return 
                last_segment.position + 
                step_to_next_segment;
        }
    }

    private void add_last_segment_sticking_to_emitter(Point end_point) {
        Vector2 direction = ((Vector2)transform.position - segments.Last().position).normalized;
        add_segment(
            end_point,
            direction
        );
        
        //Debug.Log($"TRAIL: add_last_segment_sticking_to_emitter end_point={end_point}" );
    }


    private void add_segment(
        Point in_position,
        Point in_direction
    ) {
        segments.Add(
            new Segment(
                in_position,
                in_direction,
                width
            )
        );
    }

    private int new_segments_needed(
        Point goal_point,
        out Point direction_to_goal
    ) {
        Point difference = goal_point - 
                           last_segment.position;

        int segments_needed = Mathf.FloorToInt(
            difference.magnitude/distance_between_segments
        );
        direction_to_goal = difference.normalized;

        // if (segments_needed > 0) {
        //     Debug.Log(
        //         $"TRAIL: difference.magnitude={difference.magnitude} " +
        //         $"distance_between_segments={distance_between_segments} " +
        //         $"segments_needed={segments_needed} "+
        //         $"goal_point={goal_point} "+
        //         $"last_segment={last_segment.position}"
        //     );
        // }
        
        return segments_needed;
    }


    private float last_decorative_update;
    
    private static readonly int shader_color = Shader.PropertyToID("_Color");
    private static readonly int shader_alpha = Shader.PropertyToID("_Alpha");
    private static readonly int shader_start_time = Shader.PropertyToID("_Start_time");
    private float decorative_delta_time{
        get{
            return Time.time - last_decorative_update;
        }
    }
    private void decorative_update() {
        alpha = get_faded_alpha();
        if (alpha <= 0) {
            disappear();
            return;
        }
        apply_alpha(alpha);

        last_decorative_update = Time.time;
    }


    private void add_new_segments_to_mesh() {
        mesh.vertices = get_vertices_from_segments();
        mesh.uv = get_main_uvs_from_segments();
        mesh.triangles = get_triangles_from_segments();
    }

    
    private Vector3[] get_vertices_from_segments() {
        vertices.Clear();
        foreach(Segment segment in segments) {
            vertices.Add(segment.left_point);
            vertices.Add(segment.right_point);
        }
        return vertices.ToArray();
    }


    
    private Vector2[] get_main_uvs_from_segments() {
        main_uvs.Clear();
        main_uvs.Add(new Point(0, 1));
        main_uvs.Add(new Point(0, 0));
        
        float previous_texture_x = 0;
        for (int i_segment = 1; i_segment < segments.Count; i_segment += 1) {
            var texture_progression = 
                (
                    segments[i_segment-1].position - 
                    segments[i_segment].position
                ).magnitude / 
                texture_length;

            var this_texture_x = previous_texture_x + texture_progression;
            
            main_uvs.Add(new Point(this_texture_x, 1));
            main_uvs.Add(new Point(this_texture_x, 0));
            previous_texture_x = this_texture_x;
        }
        return main_uvs.ToArray();
    }

    private int[] get_triangles_from_segments() {
        Contract.Requires(segments.Count >= 2, "need minimum 2 segments to draw a line between");
        triangles.Clear();
     
        for (int i_segment = 0; i_segment < segments.Count-1; i_segment+=1) {
            if (segments[i_segment].is_abruption) {
                continue;
            }
            int i_quad_start = i_segment*2;
            triangles.Add(i_quad_start);
            triangles.Add(i_quad_start+1);
            triangles.Add(i_quad_start+2);

            triangles.Add(i_quad_start+1);
            triangles.Add(i_quad_start+3);
            triangles.Add(i_quad_start+2);
        }

        return triangles.ToArray();
    }
    
    private float get_faded_alpha() {
        return (alpha - fade_speed * decorative_delta_time);
    }

    private void apply_alpha(float alpha) {
        Contract.Requires(alpha>0);
        Color old_color = mesh_renderer.material.color;
        var color = new Color(
            old_color.r,
            old_color.g,
            old_color.b,
            alpha
        );
        mesh_renderer.material.SetColor(shader_color, color);
        mesh_renderer.material.SetFloat(shader_alpha, alpha);
    }

  
    private void disappear() {
        CancelInvoke();
        segments.Clear();
        mesh_object.SetActive(false);
        this.enabled = false;
        on_disappeared?.Invoke();
    }

    public bool is_active() {
        return segments.Count > 0;
    }
    
    private void debug_draw_segmnents() {
        foreach(Segment segment in segments) {
            
            Gizmos.color = Color.green;
            Gizmos.DrawLine(segment.left_point, segment.position);
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(segment.right_point, segment.position);
        }
    }

    private void OnDrawGizmos() {
        debug_draw_segmnents();
    }
}
}