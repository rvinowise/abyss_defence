using UnityEngine;
using rvinowise.unity.extensions;

using rvinowise.unity.geometry2d;
using static rvinowise.unity.geometry2d.Directions;



namespace rvinowise.unity.units.parts.limbs{

public class Turning_element: Component_creator {
    
    [SerializeField]
    public Transform rotated_bone;

    public Span possible_span;
    public float rotation_speed;
    public float rotation_acceleration = 550f;
    public float rotation_friction = 0f;

    
   
    public Degree current_rotation_inertia;
    public Saved_physics last_physics = new Saved_physics();
    
    public Quaternion target_quaternion {
        get { return target_direction.rotation;}
        set { target_direction.rotation = value; }
    }
    public Relative_direction target_direction;
    
    public Quaternion rotation {
        get {
            return rotated_bone.rotation;
        }
        set {
            rotated_bone.rotation = value;    
        }
    }


    protected virtual void Awake() {
        //base.Awake();
        if (rotated_bone == null) { // if not specified in the Editor - by-default rotate itself
            rotated_bone = this.transform;
        }
    }


    public Relative_direction set_target_direction_relative_to_parent(Quaternion in_rotation) {
        target_direction = new Relative_direction(
            in_rotation, transform.parent
        );
        return target_direction;
    }
    public Relative_direction set_target_direction_relative_to_parent(float in_degrees) {
        target_direction = new Relative_direction(
            in_degrees, transform.parent
        );
        return target_direction;
    }

    public virtual void rotate_to_desired_direction_FAST() {
        /*var parent = rotated_bone.parent.gameObject.GetComponent<Turning_element>();
        if (parent) {
            Quaternion rotation_from_parent = Quaternion.Inverse(parent.rotation) * rotated_bone.rotation;
            Quaternion final_default_rotation = parent.desired_direction * rotation_from_parent;
            Quaternion rotation_to_destination = Quaternion.Inverse(final_default_rotation) * target_direction.rotation;
            Quaternion current_needed_rotation = rotated_bone.rotation * rotation_to_destination;
            rotated_bone.rotate_to(current_needed_rotation, rotation_speed);
        } else {*/
        //rotated_bone.rotate_to(target_direction, rotation_speed);
        //}
    }

    public virtual void rotate_to_desired_direction() {

        float angle_to_pass = rotated_bone.rotation.degrees_to(target_direction.rotation).degrees;
        rvinowise.rvi.contracts.Contract.Assume(Mathf.Abs(angle_to_pass) < 180f, "angle too big");
        
        if (!has_reached_target()) 
        {
            if (is_moving_towards_target()) {
                float needed_time = Mathf.Abs(angle_to_pass / current_rotation_inertia.degrees);
                float time_to_stopping =
                    Mathf.Abs(current_rotation_inertia.degrees) /
                    rotation_acceleration;
                if (is_ready_to_reach_target()) {
                    fix_at_target_direction();
                }
                else {
                    if (moving_towards_target_too_fast(time_to_stopping, needed_time)) {
                        slow_down_approaching_the_target();
                    }
                    else {
                        this.current_rotation_inertia = get_incresed_speed_towards_target(angle_to_pass);
                    }
                }
            }
            else {
                this.current_rotation_inertia = get_incresed_speed_towards_target(angle_to_pass);
            }
        }
        
        this.update_rotation();

        
        bool has_reached_target() {
            return Mathf.Abs(angle_to_pass) < 0.01f;
        }
        
        bool moving_towards_target_too_fast(float time_to_stopping, float needed_time) {
            return time_to_stopping >= needed_time;
        }

        bool is_ready_to_reach_target() {
            return (
                (
                    Mathf.Abs(angle_to_pass) < Mathf.Abs(current_rotation_inertia.degrees)  * Time.deltaTime
                ) &&
                rotation_acceleration >= Mathf.Abs(current_rotation_inertia.degrees)
            );
        }
        void fix_at_target_direction() {
            rotated_bone.rotation = target_direction.rotation;

            current_rotation_inertia = Degree.zero;
        }
        
        bool is_moving_towards_target() {
            return (int)Mathf.Sign(current_rotation_inertia.degrees) == (int)Mathf.Sign(angle_to_pass);
        }
        
        void slow_down_approaching_the_target() {
            current_rotation_inertia = current_rotation_inertia.change_magnitude_by_degrees(
                -rotation_acceleration * Time.deltaTime
            );
        }

        Degree get_incresed_speed_towards_target(float angle_to_target) 
        {
            return new Degree(
                this.current_rotation_inertia.degrees + 
                (rotation_acceleration * Mathf.Sign(angle_to_target) * Time.deltaTime) 
            );
        }
    }


    public virtual void jump_to_desired_direction() {
        rotated_bone.rotation = target_direction.rotation;
    }


    public bool at_desired_rotation() {
        return this.rotation.close_enough_to(this.target_quaternion);
    }


    public void collide_with_rotation_borders() {
        float delta_degrees = rotated_bone.parent.delta_degrees(rotated_bone);
        if (delta_degrees > possible_span.max) {
            rotated_bone.rotation = rotated_bone.parent.rotation * degrees_to_quaternion(possible_span.max);
            if (current_rotation_inertia.use_minus().degrees > 0) {
                current_rotation_inertia = Degree.zero;
            }
        }
        else if (delta_degrees < possible_span.min) {
            rotated_bone.rotation = rotated_bone.parent.rotation * degrees_to_quaternion(possible_span.min);
            if (current_rotation_inertia.use_minus().degrees < 0) {
                current_rotation_inertia = Degree.zero;
            }
        }
        
    }

    public void rotate_to(Quaternion direction) {
        transform.rotate_to(direction, rotation_speed );
    }

    protected virtual void update_rotation() {
        Degree partial_rotation = current_rotation_inertia * Time.deltaTime;
        rotated_bone.rotation = (Degree.from_quaternion(rotated_bone.rotation) + partial_rotation).to_quaternion();
        current_rotation_inertia = current_rotation_inertia.change_magnitude_by_degrees(
            -rotation_friction * Time.deltaTime
        );
    }

    public class Strategy : Headspring.Enumeration<Strategy, int> {
        public static readonly Strategy Reach_desired_direction = new Strategy(0, "Reach_desired_direction");
        public static readonly Strategy Controlled_from_outside = new Strategy(1, "Controlled_from_outside");
        private Strategy(int value, string displayName) : base(value, displayName) { }
    }
}
}