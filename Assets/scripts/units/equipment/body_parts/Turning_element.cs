using UnityEngine;
using rvinowise.unity.extensions;

using rvinowise.unity.geometry2d;
using static rvinowise.unity.geometry2d.Directions;
using rvinowise.unity.units.parts.limbs;
using UnityEditor;

namespace rvinowise.unity.units.parts{

public class Turning_element: MonoBehaviour {
    
    
    public Span possible_span;
    public float rotation_speed;
    public float rotation_acceleration = 550f;
    public float rotation_friction = 0f;

    
   
    public Degree current_rotation_inertia;
    public Saved_physics last_physics = new Saved_physics();
    
    public Quaternion rotation {
        get {
            return transform.rotation;
        }
        set {
            transform.rotation = value;
        }
    }
    public virtual Quaternion target_rotation{get;set;}
    public virtual Degree target_degree{
        get{
            return target_rotation.to_degree();
        }
        set{
            target_rotation = value.to_quaternion();
        }
    }
    public bool target_direction_relative = false;
    
    public Degree local_degrees {
        get {
            return transform.local_degrees();
        }
    }

    public Vector3 position => transform.position;
    public Vector3 localPosition {
        get=>transform.localPosition;
        set=>transform.localPosition = value;
    }
    public void direct_to(Vector2 aim) => transform.direct_to(aim);
    public void set_direction(float direction) => transform.set_direction(direction);

    protected virtual void Awake() {
        possible_span.init_for_direction(this.local_degrees);
    }

    public Quaternion set_target_direction_relative_to_parent(Quaternion in_rotation) {
        target_rotation = in_rotation;
        target_direction_relative = true;
        return target_rotation;
    }
    public Quaternion set_target_direction_relative_to_parent(float in_degrees) {
        target_rotation = Directions.degrees_to_quaternion(
            in_degrees
        );
        target_direction_relative = true;
        return target_rotation;
    }


    public virtual void rotate_to_desired_direction_FAST() {
        /*var parent = parent.gameObject.GetComponent<Turning_element>();
        if (parent) {
            Quaternion rotation_from_parent = Quaternion.Inverse(parent.rotation) * rotation;
            Quaternion final_default_rotation = parent.desired_direction * rotation_from_parent;
            Quaternion rotation_to_destination = Quaternion.Inverse(final_default_rotation) * target_direction.rotation;
            Quaternion current_needed_rotation = rotation * rotation_to_destination;
            rotate_to(current_needed_rotation, rotation_speed);
        } else {*/
        //rotate_to(target_direction, rotation_speed);
        //}
    }

    
    public virtual void rotate_to_desired_direction() {
        

        float angle_to_pass = rotation.degrees_to(target_rotation);
        rvinowise.contracts.Contract.Assume(Mathf.Abs(angle_to_pass) < 180f, "angle too big");
        
        if (!has_reached_target())
        {
            if (is_ready_to_reach_target()) {
                fix_at_target_direction();
            } else {
                
                
                
                float optimal_speed = get_optimal_speed_towards_target(
                    Mathf.Abs(angle_to_pass),
                    rotation_acceleration
                );
                var rotation_side = Side.from_degrees(angle_to_pass);
                change_rotation_speed(
                    rotation_side,
                    optimal_speed
                );
                
            }
        }
        
        
        this.update_rotation();

        #region inner functions
        bool has_reached_target() {
            return Mathf.Abs(angle_to_pass) < 0.01f;
        }
        
        bool is_ready_to_reach_target() {
            bool result = (
                (
                    Mathf.Abs(angle_to_pass) < Mathf.Abs(current_rotation_inertia)  * Time.deltaTime
                ) 
                &&
                (
                    rotation_acceleration* Time.deltaTime/4 >= Mathf.Abs(current_rotation_inertia)* Time.deltaTime
                )
            );
            if ((result)&&(gameObject.name == "human")) {
                var test = Time.deltaTime;
                rvinowise.unity.debug.Debug.DrawLine_simple(
                    transform.position, 
                    transform.position + transform.rotation*Vector2.right,
                    Color.green,
                    3f,
                    1f
                );
            }
            return result;
        }

        void fix_at_target_direction() {
            rotation = target_rotation;
            current_rotation_inertia = Degree.zero;
        }
        
        #endregion
    }

    private float get_optimal_speed_towards_target(
        float angle_to_pass,
        float rotation_acceleration
    ) {
        /* if (angle_to_pass < rotation_acceleration) {
            return rotation_acceleration;
        } */
        return Mathf.Sqrt(angle_to_pass * rotation_acceleration);
    }

    private void change_rotation_speed(
        Side rotation_direction,
        float optimal_speed
    ) {
        optimal_speed = optimal_speed * rotation_direction.Value;
        float difference = optimal_speed - current_rotation_inertia.degrees;
        

        var speed_change = rotation_acceleration * Mathf.Sign(difference) * Time.deltaTime;

        if (Mathf.Abs(difference) <= Mathf.Abs(speed_change) ) {
            current_rotation_inertia = optimal_speed;
        } else {
            
            current_rotation_inertia += speed_change;
        }
    }

    public virtual void jump_to_desired_direction() {
        rotation = target_rotation;
    }


    public bool at_desired_rotation() {
        return this.rotation.close_enough_to(this.target_rotation);
    }


    public void preserve_possible_rotations() {
        if (!is_within_span()) {
            if (this is Segment segment) {
                segment.debug_draw_line(Color.red,1);
            }
            collide_with_closest_border();
        }
    }

    private void collide_with_closest_border() {
        var parent = transform.parent;
        Degree abs_min_border = parent.rotation * possible_span.min;
        Degree abs_max_border = parent.rotation * possible_span.max;
        bool touches_min_border = 
            Mathf.Abs(abs_min_border.angle_to(this.rotation)) >
            Mathf.Abs(abs_max_border.angle_to(this.rotation));
        if (touches_min_border) {
            rotation = parent.rotation * degrees_to_quaternion(possible_span.max);
        } else {
            rotation = parent.rotation * degrees_to_quaternion(possible_span.min);
        }
        if (moves_into_border()) {
            current_rotation_inertia = Degree.zero;
        }

        bool moves_into_border() {
            bool direct_result = false;
            if ((touches_min_border)&&(current_rotation_inertia.use_minus() > 0)) {
                direct_result = true;
            } else if ((!touches_min_border)&&(current_rotation_inertia.use_minus() < 0)) {
                direct_result = true;
            }
            return direct_result && !possible_span.goes_through_switching_degrees;
        }
    }


    protected virtual void update_rotation() {
        Degree rotation_change = current_rotation_inertia * Time.deltaTime;
        rotation = (Degree.from_quaternion(rotation) + rotation_change).to_quaternion();
  
    }

    public bool is_within_span() {
        float delta_degrees = transform.parent.delta_degrees(transform);
        bool is_within_smaller_span = false;
        if (
            (delta_degrees > possible_span.min)&&
            (delta_degrees < possible_span.max)
        ) 
        {
            is_within_smaller_span = true;
        }
        if (possible_span.goes_through_switching_degrees) {
            return !is_within_smaller_span;
        }
        return is_within_smaller_span;
    }

    public class Strategy : Headspring.Enumeration<Strategy, int> {
        public static readonly Strategy Reach_desired_direction = new Strategy(0, "Reach_desired_direction");
        public static readonly Strategy Controlled_from_outside = new Strategy(1, "Controlled_from_outside");
        private Strategy(int value, string displayName) : base(value, displayName) { }
    }

    protected virtual void OnDrawGizmos() {
        float line_length = 0.1f;
        Gizmos.color = Color.green;
        
        var parent_rotation = Quaternion.identity;
        if (transform.parent != null){
            parent_rotation = transform.parent.transform.rotation;
        }
        var min_rotation = parent_rotation * Directions.degrees_to_quaternion(possible_span.min);
        var max_rotation = parent_rotation * Directions.degrees_to_quaternion(possible_span.max);
        Gizmos.DrawLine(transform.position, transform.position+min_rotation * Vector2.right * line_length);
        Gizmos.DrawLine(transform.position, transform.position+max_rotation * Vector2.right * line_length);
    }
}
}