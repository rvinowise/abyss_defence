using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using rvinowise.unity.extensions;


using rvinowise.unity.geometry2d;
using rvinowise.rvi.contracts;
using Point = UnityEngine.Vector2;
using Quaternion = UnityEngine.Quaternion;
using Random = UnityEngine.Random;

namespace rvinowise.unity.effects.trails.mesh_impl {

public class Smoke_trail_TEST:
MonoBehaviour,
ITrail 
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
    private List<Vector2> uvs = new List<Vector2>();
    private Mesh mesh;
    private float alpha;

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
        /* Contract.Requires(
            segments.Count == 2, 
            "starting points shound be given by the creater of the trail before its Enabling"
        ); */

        set_alpha(1);
        mesh_renderer.material.SetFloat("_Start_time", Time.time);
        mesh_object.SetActive(true);
        mesh.Clear();
        
        last_decorative_update = Time.time;
        //InvokeRepeating("decorative_update", decorative_update_frequency, decorative_update_frequency);


        add_segment(new Point(0,0), new Point(0,1));
        add_segment(new Point(0,1), new Point(0,1));
        add_segment(new Point(0,2), new Point(0,1));
        add_segment(new Point(0,3), new Point(0,1));
        add_segment(new Point(0,4), new Point(0,1));
        add_segment(new Point(0,5), new Point(0,1));
        
        mesh.vertices = get_vertices_from_segments();
        mesh.triangles = get_triangls_from_segments();
        
        /* mesh.uv = new Point[] {
            new Point(0,1), new Point(0,0),
            new Point(0.2f,1), new Point(0.2f,0),
            new Point(0.4f,1), new Point(0.4f,0),
            new Point(6,1), new Point(6,0),
            new Point(7,1), new Point(7,0),
            new Point(8,1), new Point(8,0),
        }; */
        mesh.uv = new Point[] {
            new Point(0,1), new Point(0,0),
            new Point(1,1), new Point(1,0),
            new Point(2,1), new Point(2,0),
            new Point(3,1), new Point(3,0),
            new Point(4,1), new Point(4,0),
            new Point(5,1), new Point(5,0),
        };
    }

    
    public void init_first_points(
        Vector2 start_position,
        Vector2 in_direction = new Vector2()
    ) {
        /* add_segment(
            start_position,
            in_direction
        );
        add_last_segment_sticking_to_emitter(start_position); */
        
    }

    public void add_bend_at(Vector2 position)
    {
        build_trail_necessarily_reaching_point(position);
        
        apply_new_segments_to_mesh();
    }
    public void add_bend_at(
        Vector2 position,
        Vector2 direction
    )
    {
        Contract.Requires(direction.magnitude == 1, "direction should be a normalized vector");
        build_trail_necessarily_reaching_point(position);
        
        Segment segment = new Segment(
            position,
            direction,
            width*2
        );
        segments.Add(segment); 
        add_last_segment_sticking_to_emitter(position);

        //Debug.Break();

        apply_new_segments_to_mesh();
    }

    public bool has_visible_parts()
    {
        return alpha > 0;
    }

    
    

    private bool build_trail_necessarily_reaching_point(Point to_point) {
        if (last_segment.position == to_point) {
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

        add_segment(
            to_point,
            direction_to_goal
        );

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
            segments_needed-1,
            direction_to_goal
        );

        add_last_segment_sticking_to_emitter(emitter.position);
        return true;
    }

    private void build_trail_upto_point(
        Point end_point,
        int segments_needed,
        Vector2 direction_to_goal
    ) {
        //detach_last_point_from_emitter();
        
        add_segments_in_direction(
            segments_needed-1,
            direction_to_goal
        );

        add_last_segment_sticking_to_emitter(end_point);
    }

    private void detach_last_point_from_emitter() {
        last_segment_settled = true;
        /* Point last_settled_moving_vector = madeup_segment_moving_vector(
            moving_vectors
        );
        segments[segments.Count-1].moving_vector = last_settled_moving_vector; */
    }

    private void add_segments_in_direction(
        int segments_n,
        Point direction_vector
    ) {
        Point step_to_next_segment = direction_vector * distance_between_segments;

        for (int i=0; i<segments_n; i++) {
            Point next_position = segments.Last().position + step_to_next_segment;
            add_segment(
                next_position,
                direction_vector
            );
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
        Point difference = goal_point - last_settled_segment.position;

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


    protected void Update() {
        /* bool trail_grew = built_trail_compensating_flight_of_emitter();
        bool end_changed = move_end_of_trail_to_emitter();
        if (trail_grew) {
            add_new_segments_to_mesh();
        } else if (end_changed) {
            mesh.vertices = get_vertices_from_segments();
        } */
        debug_draw_segmnents();
    }

    private float last_decorative_update;
    private float decorative_update_frequency = 0.1f;
    private float decorative_delta_time{
        get{
            return Time.time - last_decorative_update;
        }
    }
    private void decorative_update() {
        fade_gradually();
        if (!has_visible_parts()) {
            disappear();
        } else if (segment_speed_difference > 0) {
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
    
    private void apply_new_segments_to_mesh() {
        mesh.vertices = get_vertices_from_segments();
        mesh.uv = get_uvs_from_segments();
        mesh.triangles = get_triangls_from_segments();
    }
    
    private Vector3[] get_vertices_from_segments() {
        vertices.Clear();
        foreach(Segment segment in segments) {
            vertices.Add((Vector3)segment.left_point);
            vertices.Add((Vector3)segment.right_point);
        }
        return vertices.ToArray();
    }

    private Vector2[] get_uvs_from_segments() {
        uvs.Clear();
        for (int i_segment = 0; i_segment < segments.Count; i_segment += 1) {
            uvs.Add(new Point(i_segment,1));
            uvs.Add(new Point(i_segment,0));
        }
        Debug.Log("get_uvs_from_segments()");
        return uvs.ToArray();
    }

    private int[] get_triangls_from_segments() {
        //Contract.Requires(segments_n >= 2, "need minimum 2 segments to draw a line between");
        triangles.Clear();
        for (int i_segment = 0; i_segment < segments.Count-1; i_segment+=1) {
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
    

    private bool move_end_of_trail_to_emitter() {
        if (
            unsettled_segment.position != (Vector2)emitter.position
        ) {
            Point direction_to_goal = ((Vector2)emitter.position - last_settled_segment.position).normalized;
            unsettled_segment.move_and_rotate(emitter.position, direction_to_goal);
            return true;
        }
        return false;
    }

    public float fade_speed = 1f;
    private void fade_gradually() {
        set_alpha(alpha - fade_speed * decorative_delta_time);
    }

    private void set_alpha(float in_alpha) {
        alpha = in_alpha;
        Color old_color = mesh_renderer.material.color;
        var color = new Color(
            old_color.r,
            old_color.g,
            old_color.b,
            alpha
        );
        mesh_renderer.material.SetColor("_Color", color);
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