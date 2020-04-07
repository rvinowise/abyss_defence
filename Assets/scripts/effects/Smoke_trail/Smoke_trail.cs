using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using rvinowise;
using geometry2d;
using rvinowise.rvi.contracts;
using Point = UnityEngine.Vector2;
using Quaternion = System.Numerics.Quaternion;
using Random = UnityEngine.Random;

namespace rvinowise.effects.trails {

public class Smoke_trail:MonoBehaviour {
    
    /* parameters */
    public float distance_between_segments = 1f;
    public float segment_max_speed = 0.1f;
    public float segment_speed_difference = 0.02f; 

    
    /* fields for inner functionality */
    private List<Segment> segments = new List<Segment>();
    private Vector3[] vertices;
    private List<int> indices = new List<int>(/*(segments_n-1) * 6*/);

    private Point object_start_position;
    private GameObject mesh_object;/* needed because the mesh uses global coordinates */
    private Point end {
        get { return gameObject.transform.position; }
    }
    
    /* components */
    public MeshFilter mesh_filter;
    public MeshRenderer mesh_renderer;

    protected void Awake() {
        init_components();
        object_start_position = new Point(0f,-1f);
        //build_shape();
        build_between_points();
        init_mesh_renderer(
            mesh_renderer
        );
    }

    protected void init_components() {
        mesh_object = new GameObject();
        mesh_filter = mesh_object.add_component<MeshFilter>();
        mesh_renderer = mesh_object.add_component<MeshRenderer>();
    }
    
    private void build_new_segments() {
        int segments_needed = new_segments_needed();
        if (segments_needed == 0) {
            return;
        }
        
        

        add_segments(segments, end, segments_needed);
        
        init_mesh_filter(
            mesh_filter,
            get_vertices_from_segments(segments),
            init_triangle_indices(segments_needed)
        );
        
    }

    public void build_between_points() {
        //int segments_n = (int)((start - end).magnitude/distance_between_segments)+1;
        int segments_n = new_segments_needed();
        segments = init_segments(object_start_position, gameObject.transform.position, segments_n);
        
        init_mesh_filter(
            mesh_filter,
            get_vertices_from_segments(segments),
            init_triangle_indices(segments_n)
        );

        
    }


    private int new_segments_needed() {
        Point start = segments.Any() ? 
            segments.Last().position : 
            (Vector2) object_start_position;
        
        int segments_needed = Mathf.FloorToInt(
            (start - end).magnitude/distance_between_segments
        );
        return segments_needed;
    }

    private List<Segment> add_segments(List<Segment> segments, Vector2 end, int segments_n) {
        Point start = segments.Last().left_point;
        Point direction_vector = (end - start).normalized;
        Point moving_direcition = direction_vector.rotate(90f);
        for (int i_segment = 1; i_segment <= segments_n; i_segment++) {
            float segment_offset = i_segment * distance_between_segments;

            Point position = start + direction_vector * segment_offset;
            Point moving_vector = madeup_segment_moving_vector(
                segments, 
                moving_direcition
            );
            
            segments.Add(
                new Segment(
                    position,
                    direction_vector,
                    moving_vector
                )
            );
        }
        vertices = new Vector3[segments.Count * 2];
        return segments;
    }

    private List<Segment> init_segments(Vector2 start, Vector2 end, int segments_n) {
        Point direction_vector = (end - start).normalized;
        Point moving_direcition = direction_vector.rotate(90f);
        segments.Capacity = segments_n;
        
        for (int i_segment = 0; i_segment < segments_n; i_segment++) {
            float segment_offset = i_segment * distance_between_segments;

            Point position = start+direction_vector * segment_offset;
            Point moving_vector = madeup_segment_moving_vector(
                segments, 
                moving_direcition
            );
               
            
            segments.Add(
                new Segment(
                    position,
                    direction_vector,
                    moving_vector
                )
            );
        }
        vertices = new Vector3[segments.Count * 2];
        return segments;
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

    private List<int> init_triangle_indices(int segments_n) {
        //Contract.Requires(segments_n >= 2, "need minimum 2 segments to draw a line between");
        for (int i_segment = 0; i_segment < segments_n-1; i_segment ++) {
            int i_rect_start = i_segment * 2;
            indices.Add(i_rect_start);
            indices.Add(i_rect_start+3);
            indices.Add(i_rect_start+1);

            indices.Add(i_rect_start);
            indices.Add(i_rect_start+2);
            indices.Add(i_rect_start+3);
        }

        return indices;
    }

    private void init_mesh_filter(
        MeshFilter out_mesh_filter,
        Vector3[] in_vertices,
        List<int> in_triangles
    ) {
        out_mesh_filter.mesh = new Mesh() {
            vertices = in_vertices,
            triangles = in_triangles.ToArray()
        };
        
        
    }

    private void init_mesh_renderer(MeshRenderer mesh_renderer) {
        Material material = Resources.Load<Material>("materials/smoke");
        mesh_renderer.material = material;
    }


    protected void Update() {
        move_segments();
        build_new_segments();
        apply_segments_to_mesh();
        fade_color();
    }

    public float fade_speed = 1f;
    private void fade_color() {
        var material = mesh_renderer.material;
        Color color = material.color;
        color.a -=  fade_speed * Time.deltaTime;
        material.color = color ;
    }

    private void move_segments() {
        for (int i_segment = 0; i_segment < segments.Count; i_segment++) {
            segments[i_segment].move();
        }
    }

    

    private void apply_segments_to_mesh() {
        mesh_filter.mesh.vertices = get_vertices_from_segments(segments);
    }

    
    private Vector3[] get_vertices_from_segments(List<Segment> segments) {
        //Vector3[] vertices = new Vector3[segments.Count*2];
        for (int i_segment = 0; i_segment < segments.Count; i_segment++) {
            vertices[i_segment*2] = (Vector3)segments[i_segment].points[0];
            vertices[i_segment*2+1] = (Vector3)segments[i_segment].points[1];
            
            //Debug.DrawLine(vertices[i_segment*2], vertices[i_segment*2+1], Color.green, 1f);
        }
        return vertices;
    } 
}
}