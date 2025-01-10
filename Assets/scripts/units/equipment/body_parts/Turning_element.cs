using System;
using UnityEngine;
using rvinowise.unity.extensions;

using rvinowise.unity.geometry2d;
using static rvinowise.unity.geometry2d.Directions;

namespace rvinowise.unity {

public class Turning_element: MonoBehaviour {
    
    
    public Span possible_span;
    public float rotation_acceleration = 550f;
    public float rotation_slowing = 1100f;

   
    public Degree current_rotation_inertia;

    public Quaternion rotation=> transform.rotation;
    

    public Quaternion get_target_rotation() {
        return target_rotation;
    }

    public void set_target_rotation(Quaternion in_rotation) {
        if (target_direction_relative) {
            target_rotation = transform.parent.rotation * in_rotation;
        }
        else {
            target_rotation = in_rotation;
        }
    }

    private Quaternion target_rotation;
 
    public Degree target_degree{
        get{
            return get_target_rotation().to_degree();
        }
        set{
            set_target_rotation(value.to_quaternion());
        }
    }


    public Turning_element turning_parent;

    public bool target_direction_relative;
    private bool has_turning_parent;
    private bool has_span;
    
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

    public bool ignore_parent = true;
    public void direct_to(Vector2 aim) => transform.direct_to(aim);
    public void set_direction(float direction) => transform.set_direction(direction);

    protected virtual void Awake() {
        possible_span.init_for_direction(this.local_degrees);
        if (
            (turning_parent == null) &&
            (transform.parent != null) && 
            (transform.parent.GetComponent<Turning_element>() is { } turning_element)
         ) {
            turning_parent = turning_element;
            has_turning_parent = true;
        }

        if (Math.Abs(possible_span.min.angle_to(possible_span.max)) > Turning_element.rotation_epsilon) {
            has_span = true;
        }
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

    public const float rotation_epsilon = 0.01f;


    public void rotate_towards(Quaternion target_rotation) {
        set_target_rotation(target_rotation);
        rotate_to_desired_direction();
    }
    public void rotate_to_desired_direction() {

        float angle_to_pass = get_angle_to_pass();
        rvinowise.contracts.Contract.Assume(Mathf.Abs(angle_to_pass) <= 180f, "angle too big");
        
        if (!has_reached_target())
        {
            if (is_ready_to_reach_target()) {
                fix_at_target_direction();
            } else {
                float optimal_speed = get_optimal_speed_towards_target(
                    Mathf.Abs(angle_to_pass),
                    rotation_slowing
                );
                var side_to_target = Side.from_degrees(angle_to_pass);
                change_rotation_speed(
                    side_to_target,
                    optimal_speed
                );
            }
        }
        
        
        this.update_rotation();

        #region inner functions
        bool has_reached_target() {
            return Mathf.Abs(angle_to_pass) <= rotation_epsilon;
        }
        
        bool is_ready_to_reach_target() {
            bool result = 
                (
                    (
                        Mathf.Abs(angle_to_pass) < Mathf.Abs(current_rotation_inertia)  * Time.deltaTime
                    ) 
                    &&
                    (
                        rotation_acceleration* Time.deltaTime >= Mathf.Abs(current_rotation_inertia)* Time.deltaTime
                    )
                );
            
            return result;
        }

        void fix_at_target_direction() {
            if (is_parent_taken_into_account()) {
                transform.localRotation = 
                    new Degree(
                        turning_parent.target_degree.angle_to(target_degree)
                    ).to_quaternion();
            } else {
                transform.rotation = target_rotation;
            }
            current_rotation_inertia = Degree.zero;
        }
        
        #endregion
    }

    private Quaternion rotation_when_parent_reaches_its() {
        if (is_parent_taken_into_account()) {
            return turning_parent.target_rotation * transform.localRotation;
        }
        return transform.rotation;
    }

    private bool is_parent_taken_into_account() {
        return (!ignore_parent && has_turning_parent);
    }
    private bool are_angles_restricted_by_parent() {
        return has_span;
    }
    private float get_angle_to_pass() {
        return rotation_when_parent_reaches_its().degrees_to(target_rotation);
    }

    private float get_optimal_speed_towards_target(
        float angle_to_pass,
        float rotation_slowing
    ) {
        /* if (angle_to_pass < rotation_acceleration) {
            return rotation_acceleration;
        } */
        return Mathf.Sqrt(angle_to_pass * rotation_slowing);
    }

    public void change_rotation_speed(
        Side_type side_to_target,
        float optimal_speed
    ) {
        optimal_speed = optimal_speed * (float) side_to_target;
        float difference = optimal_speed - current_rotation_inertia.degrees;
        bool needs_to_accelerate = 
            (current_rotation_inertia.side() == Side_type.NONE)
            ||
            (side_to_target == current_rotation_inertia.side())
            &&
            (Mathf.Abs(current_rotation_inertia) < Mathf.Abs(optimal_speed));

        float speed_change;
        if (needs_to_accelerate) {
            speed_change = rotation_acceleration * Mathf.Sign(difference) * Time.deltaTime;
        } else {
            speed_change = rotation_slowing * Mathf.Sign(difference) * Time.deltaTime;
        }

        if (Mathf.Abs(difference) <= Mathf.Abs(speed_change) ) {
            current_rotation_inertia = optimal_speed;
        } else {
            
            current_rotation_inertia += speed_change;
        }
    }


    public virtual void jump_to_desired_direction() {
        transform.rotation = target_rotation;
    }


    public bool at_desired_rotation() {
        return Math.Abs(rotation.degrees_to(target_rotation)) <= rotation_epsilon;
    }


    public void preserve_possible_rotations() {
        if (!is_within_span()) {
            collide_with_closest_border();
        }
    }

    private void collide_with_closest_border() {
        var parent = transform.parent;
        Degree abs_min_border = parent.rotation * new Degree(possible_span.min);
        Degree abs_max_border = parent.rotation * new Degree(possible_span.max);
        bool touches_max_border = 
            Mathf.Abs(abs_min_border.angle_to(this.rotation)) >
            Mathf.Abs(abs_max_border.angle_to(this.rotation));
        if (touches_max_border) {
            transform.rotation = parent.rotation * degrees_to_quaternion(possible_span.max);
        } else {
            transform.rotation = parent.rotation * degrees_to_quaternion(possible_span.min);
        }
        if (moves_into_border()) {
            current_rotation_inertia = Degree.zero;
        }

        bool moves_into_border() {
            bool direct_result = false;
            if ((touches_max_border)&&(current_rotation_inertia.use_minus() > 0)) {
                direct_result = true;
            } else if ((!touches_max_border)&&(current_rotation_inertia.use_minus() < 0)) {
                direct_result = true;
            }
            return direct_result && !possible_span.goes_through_switching_degrees;
        }
    }


    public virtual void update_rotation() {
        Degree rotation_change = current_rotation_inertia * Time.deltaTime;
        transform.rotation = (Degree.from_quaternion(rotation) + rotation_change).to_quaternion();
        if (are_angles_restricted_by_parent()) {
            preserve_possible_rotations();
        }
    }

    public bool is_within_span() {
        float delta_degrees = transform.parent.delta_degrees(transform);
        bool is_within_smaller_span = (delta_degrees >= possible_span.min)&&
                                      (delta_degrees <= possible_span.max);
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

#if UNITY_EDITOR
    protected virtual void OnDrawGizmos() {
        draw_span();
        draw_target_rotation();
    }
#endif
    
    private void draw_span() {
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
    
    private void draw_target_rotation() {
        float line_length = 0.2f;
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, transform.position+get_target_rotation() * Vector2.right * line_length);
    }

}
}