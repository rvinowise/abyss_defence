using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using rvinowise.unity.extensions;


using rvinowise.unity.geometry2d;
using rvinowise.contracts;
using Point = UnityEngine.Vector2;
using Quaternion = UnityEngine.Quaternion;
using Random = UnityEngine.Random;

namespace rvinowise.unity.effects.trails.mesh_impl {

public class Smoke_trail:
MonoBehaviour
{
    
    /* parameters */
    public float distance_between_segments = 1f;
    public float segment_max_speed = 0.1f;
    public float segment_speed_difference = 0.02f; 
    public float width = 0.2f;

    public UnityEngine.Events.UnityEvent on_disappeared;
    public Transform emitter;
    public GameObject mesh_object;/* needed because the mesh uses global coordinates */

    private MeshFilter mesh_filter;

    private MeshRenderer mesh_renderer;

    
    /* fields for inner functionality */
    public List<Segment> segments = new List<Segment>();
    private Segment last_settled_segment {get{
        if (last_segment_settled) {
            return segments.Last();
        }
        return segments[segments.Count-2];
    }}

    private Segment unsettled_segment {
        get {
            if (last_segment_settled) {
                return null;
            }
            return segments.Last();
        }
    }
    private bool last_segment_settled;

    private Segment last_segment {get{
        return segments.Last();
    }}
    private List<Vector3> vertices = new List<Vector3>();
    private List<int> triangles = new List<int>(/*(segments_n-1) * 6*/);
    private List<Vector2> main_uvs = new List<Vector2>();
    private List<Vector2> noise_uvs = new List<Vector2>();
    private Mesh mesh;
    private float alpha;
    private float texture_length = 2;
    
    protected void Awake() {
        init_components();
        mesh_object.transform.SetParent(null, false);
        mesh_object.transform.position = Vector3.zero;
        mesh_object.transform.rotation = Quaternion.identity;

        
    }
    protected void init_components() {
        mesh_filter = mesh_object.GetComponent<MeshFilter>();
        mesh_renderer = mesh_object.GetComponent<MeshRenderer>();
        mesh = mesh_filter.mesh;
        mesh.MarkDynamic();

        if (emitter == null) {
            emitter = transform;
        }
    }

    void OnEnable() {
        Contract.Requires(
            segments.Count == 2, 
            "starting points shound be given by the creater of the trail before its Enabling"
        );

        alpha = 1;
        apply_alpha(alpha);
        mesh_renderer.material.SetFloat("_Start_time", Time.time);
        mesh_object.SetActive(true);
        mesh.Clear();
        add_new_segments_to_mesh();
        
        last_decorative_update = Time.time;
        InvokeRepeating("decorative_update", decorative_update_frequency, decorative_update_frequency);
    }

    
    public void init_first_points(
        Vector2 start_position,
        Vector2 in_direction = new Vector2()
    ) {
        add_segment(
            start_position,
            in_direction
        );
        add_last_segment_sticking_to_emitter(start_position);

        
        /* unsettled_segment = new Segment(
            start_position,
            Vector2.one
        ); */
    }

    public void add_bend_at(
        Vector2 position,
        Vector2 new_direction
    )
    {
        Contract.Requires(new_direction.magnitude == 1, "direction should be a normalized vector");
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
        bool trail_grew = build_trail_necessarily_reaching_point(position);
        if (trail_grew) {
            add_new_segments_to_mesh();
        } else {
            mesh.vertices = get_vertices_from_segments();
        }
    }

    protected void Update() {
        bool trail_grew = built_trail_compensating_flight_of_emitter();
        bool end_changed = move_end_of_trail_to_emitter();
        if (trail_grew) {
            add_new_segments_to_mesh();
        } else if (end_changed) {
            mesh.vertices = get_vertices_from_segments();
        }
        debug_draw_segmnents();
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
            return false;
        }

        Point direction_to_goal;
        int segments_needed = new_segments_needed(
            to_point,
            out direction_to_goal
        );
        
        add_segments_in_direction(
            segments_needed,
            direction_to_goal
        );

        if (segments_needed == 0) {
            unsettled_segment.move_and_rotate(to_point, direction_to_goal);
        } else {
            segments.Add(
                Segment.new_continuation(
                    to_point,
                    direction_to_goal,
                    width
                )
            );
        }

        return true;
    }
    private bool built_trail_compensating_flight_of_emitter() {
        Point direction_to_goal;
        int segments_needed = new_segments_needed(
            emitter.position,
            out direction_to_goal
        );
        if (segments_needed == 0) {
            return false;
        }
        
        add_segments_in_direction(
            segments_needed,
            direction_to_goal
        );

        add_last_segment_sticking_to_emitter(emitter.position);
        return true;
    }


    private void detach_last_point_from_emitter() {
        last_segment_settled = true;
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

        unsettled_segment.move_and_rotate(
            next_position(),
            direction_vector
        );
        for (int i=1; i<segments_n; i++) {
            add_segment(
                next_position(),
                direction_vector
            );
        }

        Vector2 next_position() {
            return 
                last_settled_segment.position + 
                step_to_next_segment;
        }
    }

    private void add_last_segment_sticking_to_emitter(Point end_point) {
        Vector2 direction = ((Vector2)emitter.position - segments.Last().position).normalized;
        add_segment(
            end_point,
            direction
        );
        last_segment_settled = false;
    }


    private bool move_end_of_trail_to_emitter() {
        Point direction_to_goal = (
            (Vector2)emitter.position - 
            last_settled_segment.position
        ).normalized;
        
        return unsettled_segment.move_and_rotate(
            emitter.position, direction_to_goal
        );
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
                           last_settled_segment.position;

        int segments_needed = Mathf.FloorToInt(
            difference.magnitude/distance_between_segments
        );
        direction_to_goal = difference.normalized;
        return segments_needed;
    }

    

    private Point madeup_segment_moving_vector(List<Segment> segments, Point moving_direction) {
        Point last_vector = segments.Any() ? 
            segments.Last().moving_vector :
            new Point(0,0);
        float last_speed = last_vector.magnitude;
        float speed_change = Random.Range(-segment_speed_difference, segment_speed_difference);
        Point moving_vector =
            moving_direction *
            (
                last_speed + speed_change
            );

        return moving_vector;
    }


    

    private float last_decorative_update;
    private float decorative_update_frequency = 0.1f;
    private float decorative_delta_time{
        get{
            return Time.time - last_decorative_update;
        }
    }
    private void decorative_update() {
        alpha = get_faded_alpha();
        if (alpha < 0) {
            disappear();
            return;
        }
        apply_alpha(alpha);
        if (segment_speed_difference > 0) {
            apply_brownian_motion();
            mesh.vertices = get_vertices_from_segments();
        }

        last_decorative_update = Time.time;
    }


    private void apply_brownian_motion() {
        for (int i_segment = 0; i_segment < segments.Count; i_segment++) {
            segments[i_segment].move();
        }
    }
    
    private void add_new_segments_to_mesh() {
        mesh.vertices = get_vertices_from_segments();
        mesh.uv = get_main_uvs_from_segments();
        mesh.triangles = get_triangls_from_segments();
    }

    public void adjust_texture_at_end() {
        float distance_to_last_segment =  
        (
            segments.Last().position - 
            segments[segments.Count-2].position
        ).magnitude / 
        texture_length;

        float last_texture_x = main_uvs[main_uvs.Count-3].y + distance_to_last_segment;
        main_uvs[main_uvs.Count-2] = new Point(last_texture_x, 1);
        main_uvs[main_uvs.Count-1] = new Point(last_texture_x, 0);
        mesh.uv = main_uvs.ToArray();
    }


    
    private Vector3[] get_vertices_from_segments() {
        vertices.Clear();
        foreach(Segment segment in segments) {
            vertices.Add((Vector3)segment.left_point);
            vertices.Add((Vector3)segment.right_point);
        }
        return vertices.ToArray();
    }


    
    private Vector2[] get_main_uvs_from_segments(float ending_stretch = 4) {
        main_uvs.Clear();
        main_uvs.Add(new Point(0, 1));
        main_uvs.Add(new Point(0, 0));
        
        float texture_stretch=0;
        for (int i_segment = 1; i_segment < segments.Count-1; i_segment += 1) {
            texture_stretch = 
                (
                    segments[i_segment-1].position - 
                    segments[i_segment].position
                ).magnitude / 
                texture_length;
            
            main_uvs.Add(new Point(texture_stretch, 1));
            main_uvs.Add(new Point(texture_stretch, 0));
        }
        main_uvs.Add(new Point(texture_stretch+ending_stretch/texture_length, 1));
        main_uvs.Add(new Point(texture_stretch+ending_stretch/texture_length, 0));
        return main_uvs.ToArray();
    }
    private Vector2[] get_noise_uvs_from_segments(float ending_stretch = 4) {
        noise_uvs.Clear();
        noise_uvs.Add(new Point(0, 1));
        noise_uvs.Add(new Point(0, 0));
        float texture_step = 1f / (segments.Count-1);
        float texture_stretch = texture_step;
        for (int i_segment = 1; i_segment < segments.Count; i_segment += 1) {
            noise_uvs.Add(new Point(texture_stretch, 1));
            noise_uvs.Add(new Point(texture_stretch, 0));

            texture_stretch += texture_step;
        }
        return noise_uvs.ToArray();
    }

    private int[] get_triangls_from_segments() {
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
    

    

    public float fade_speed = 1f;
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
        mesh_renderer.material.SetColor("_Color", color);
        mesh_renderer.material.SetFloat("_Alpha", alpha);
    }

  
    private void disappear() {
        //Debug.Break();
        CancelInvoke();
        segments.Clear();
        on_disappeared?.Invoke();
        mesh_object.SetActive(false);
        this.enabled = false;
    }


    private void debug_draw_segmnents() {
        foreach(Segment segment in segments) {
            Debug.DrawLine(segment.left_point, segment.right_point, Color.red, 0f);
            Debug.DrawLine(segment.left_point, segment.position, Color.green, 0f);
            Debug.DrawLine(segment.right_point, segment.position, Color.blue, 0f);
        }
        
    }
}
}