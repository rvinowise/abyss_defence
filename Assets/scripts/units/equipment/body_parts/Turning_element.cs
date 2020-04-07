using extesions;
using rvinowise.units;
using UnityEngine;
using geometry2d;
using Headspring;
using static geometry2d.Directions;

namespace rvinowise.units.parts.limbs{

public class Turning_element: Game_object {
    
    
    public Span possible_span;
    public float rotation_speed;
    public Saved_physics last_physics = new Saved_physics();
    //public Turning_element.Strategy strategy = Strategy.Reach_desired_direction;

    public Quaternion desired_direction {
        get { return target_direction.direction;}
        set { target_direction.direction = value; }
    }
    public Relative_direction target_direction;
    
    public Turning_element():base() {
    }
    public Turning_element(string name):base(name) {
    }
    public Turning_element(string name, GameObject prefab):base(name, prefab) {
    }

    public virtual void rotate_to_desired_direction() {
        last_physics.position = transform.position;
        transform.rotate_to(target_direction, rotation_speed);
    }


    public bool at_desired_rotation() {
        return this.rotation == this.desired_direction;
    }


    public void preserve_possible_rotation() {
        float delta_degrees = parent.delta_degrees(transform);
        if (delta_degrees > possible_span.max) {
            rotation = parent.rotation * degrees_to_quaternion(possible_span.max);
        }
        else if (delta_degrees < possible_span.min) {
            rotation = parent.rotation * degrees_to_quaternion(possible_span.min);
        }
    }

    public void rotate_to(Quaternion direction) {
        transform.rotate_to(direction, rotation_speed );
    }


    public class Strategy : Headspring.Enumeration<Strategy, int> {
        public static readonly Strategy Reach_desired_direction = new Strategy(0, "Reach_desired_direction");
        public static readonly Strategy Controlled_from_outside = new Strategy(1, "Controlled_from_outside");
        private Strategy(int value, string displayName) : base(value, displayName) { }
    }
}
}