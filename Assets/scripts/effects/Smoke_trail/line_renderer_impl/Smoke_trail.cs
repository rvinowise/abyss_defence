using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using rvinowise.unity.extensions.pooling;


using Point = UnityEngine.Vector3;
using rvinowise.unity.extensions;
using rvinowise.contracts;

namespace rvinowise.unity.effects.trails.line_renderer_impl {

[RequireComponent(typeof(LineRenderer))]
public class Smoke_trail: 
MonoBehaviour,
ITrail
{

    
    

    /* parameters */
    public float distance_between_segments = 0.1f;
    public float segment_max_speed = 0.1f;
    public float segment_speed_difference = 0.02f; 
    public float segment_slowing_down = 0.1f;
    public float fade_speed = 0.02f;
    public UnityEngine.Events.UnityEvent on_disappeared;
    
    public float start_alpha = 1f;

    private Point emitter_position {get{
        return transform.position;
    }}
    private List<Point> points = new List<Point>();
    private List<Point> moving_vectors = new List<Point>();
    //private List<Point> visited_points = new List<Point>();
    private Point last_settled_point {get{
         return points[points.Count-2];
    }}
    private LineRenderer line_renderer;
    private Pooled_object pooled_object;
    private float decorative_update_frequency = 0.1f;
    private float last_decorative_update;
    private float decorative_delta_time{
        get{
            return Time.time - last_decorative_update;
        }
    }

    private float alpha;

    private readonly int _Start_time = Shader.PropertyToID("_Start_time");
    

    void Awake() {
        line_renderer = GetComponent<LineRenderer>();
        pooled_object = GetComponent<Pooled_object>();
       
    }

    void OnEnable(){
        Contract.Requires(
            points.Count == 2, 
            "starting points shound be given by the creater of the trail before its Enabling"
        );
        set_alpha(start_alpha);
        line_renderer.material.SetFloat(_Start_time, Time.time);
        apply_points_to_line_renderer(line_renderer,points);

        last_decorative_update = Time.time;
        InvokeRepeating("decorative_update", decorative_update_frequency, decorative_update_frequency);
    }



    public void init_first_points(Vector2 start_position, Vector2 direction=new Vector2()) {
        points.Add(
            start_position
        );
        points.Add(
            start_position
        );
        moving_vectors.Add(Vector3.zero);
        moving_vectors.Add(Vector3.zero);

    }


    public void visit(Point point) {
        bool trail_grew = build_trail_upto_point(point);
        if (trail_grew){
            apply_points_to_line_renderer(line_renderer, points);
        }
    }
    public void add_bend_at(Vector2 point) {
        move_last_point_to_the_position(point);
        detach_last_point_from_emitter(Point.zero);

        add_last_point_sticking_to_emitter();
        
        apply_points_to_line_renderer(line_renderer, points);
    }

    private void move_last_point_to_the_position(Point point) {
        points[points.Count-1] = point;
    }

    public bool has_visible_parts() {
        return alpha > 0f;
    }

    void Update()
    {
        bool trail_grew = build_trail_upto_emitter();
        if (trail_grew){
            apply_points_to_line_renderer(line_renderer, points);
        }
        /* if (is_time_of_decorative_update()) {
            decorative_update();
        } */
        move_end_of_trail_to_emitter();
    }

    
    private void decorative_update() {
        fade_gradually();
        if (segment_speed_difference > 0) {
            apply_brownian_motion(points, moving_vectors);
            apply_points_to_line_renderer(line_renderer, points);
        }

        last_decorative_update = Time.time;
    }


    private int lacking_trail_points() {
        Point difference = emitter_position - last_settled_point;
        int need_new_points_n = (int)Mathf.Floor(difference.magnitude / distance_between_segments);

        return need_new_points_n;
    }
    private bool build_trail_upto_emitter() {
        return build_trail_upto_point(emitter_position);
    }

    private bool build_trail_upto_point(Point goal_point) {
        Point difference = goal_point - last_settled_point;
        int need_new_points_n = (int)Mathf.Floor(
            difference.magnitude / distance_between_segments
        );
        if (need_new_points_n < 1) {
            return false;
        }

        Point direction_vector = difference.normalized;
        Point brownian_direcition = direction_vector.rotate(90f);

        detach_last_point_from_emitter(brownian_direcition);
        
        add_points_between_last_one_and_emitter(
            need_new_points_n-1,
            brownian_direcition,
            direction_vector
        );

        add_last_point_sticking_to_emitter();

        return true;
    }

    private void detach_last_point_from_emitter(Point brownian_direcition) {
        Point last_settled_moving_vector = madeup_segment_moving_vector(
            moving_vectors
        );
        moving_vectors[points.Count-1] = last_settled_moving_vector;
    }

    private void add_points_between_last_one_and_emitter(
        int points_n,
        Point brownian_direcition,
        Point direction_vector
    ) {
        Point step_to_next_point = direction_vector * distance_between_segments;

        for (int i=0; i<points_n; i++) {
            Point next_position = points.Last() + step_to_next_point;
            add_point(
                next_position,
                madeup_segment_moving_vector(
                    moving_vectors
                )
            );
        }
    }
    private void add_point(Point in_point, Point in_moving_vector) {
        points.Add(in_point);
        moving_vectors.Add(in_moving_vector);
    }
    private void add_last_point_sticking_to_emitter() {
        points.Add(emitter_position);
        moving_vectors.Add(Point.zero);
    }

    private Point madeup_segment_moving_vector(
        List<Point> moving_vectors
        
    ) {
        Point last_vector = moving_vectors.Any() ? 
            moving_vectors.Last():
            new Point(0,0);
        //float last_speed = last_vector.magnitude;
        //float speed_change = (int)(Random.RandomRange(-1,1)) * segment_speed_difference;
        /* Point moving_vector =
            moving_direction *
            (
                last_speed + speed_change
            ); */
        Point moving_vector = last_vector + (Point.one * segment_speed_difference).rotate(Random.value * 360f);
        return moving_vector;
    }
    
    private void move_end_of_trail_to_emitter() {
        points[points.Count-1] = emitter_position; //for not to be overwritten in decorative_update()
        line_renderer.SetPosition(points.Count-1, emitter_position);
    }

    private void apply_brownian_motion(
        List<Point> points,
        List<Point> moving_vectors
    )
    {
        for(int i_point=0; i_point<points.Count-2; i_point++)
        {
            Point old_point = points[i_point];
            points[i_point] = old_point+moving_vectors[i_point] * decorative_delta_time;
            moving_vectors[i_point] /= (1+(segment_slowing_down * decorative_delta_time));
        }
    }

    private IEnumerator<Point> freely_drifting_points() {
        for(int i_point=0; i_point<points.Count-2; i_point++)
        {
            yield return points[i_point];
        }
    }
        
    private void apply_points_to_line_renderer(
        LineRenderer line_renderer,
        List<Point> points
    ) {
        line_renderer.positionCount = points.Count;
        line_renderer.SetPositions(points.ToArray());
    }
    
    
    private void fade_gradually() {
        set_alpha(alpha - fade_speed * decorative_delta_time);

        if (!has_visible_parts()) {
            disappear();
        }
        
    }

    private void set_alpha(float in_alpha) {
        alpha = in_alpha;
        Color old_color = line_renderer.material.color;
        var color = new Color(
            old_color.r,
            old_color.g,
            old_color.b,
            alpha
        );
        line_renderer.material.SetColor("_Color", color);
    }

    private void disappear() {
        CancelInvoke();
        points.Clear();
        moving_vectors.Clear();
        on_disappeared?.Invoke();
        this.enabled = false;
    }

       
}

}