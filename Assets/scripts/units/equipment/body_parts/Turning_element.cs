using System.Runtime.CompilerServices;
using extesions;
using rvinowise.units;
using UnityEngine;
using geometry2d;
using Headspring;
using UnityEditor;
using static geometry2d.Directions;

namespace rvinowise.units.parts.limbs{

public class Turning_element: Component_creator {
    
    /* parameters from the editor */
    
    [SerializeField]
    public Transform rotated_bone;

    public Span possible_span;
    public float rotation_speed;

    /*  */
    
    public Saved_physics last_physics = new Saved_physics();
    
    public Quaternion desired_direction {
        get { return target_direction.direction;}
        set { target_direction.direction = value; }
    }
    public Relative_direction target_direction;

    protected void Awake() {
        //base.Awake();
        if (rotated_bone == null) { // if not specified in the Editor - by-default rotate itself
            rotated_bone = this.transform;
        }
    }

   

    public virtual void rotate_to_desired_direction() {
        rotated_bone.rotate_to(target_direction, rotation_speed);
    }
    public virtual void jump_to_desired_direction() {
        rotated_bone.rotation = target_direction.direction;
    }


    public bool at_desired_rotation() {
        return this.rotation == this.desired_direction;
    }


    public void preserve_possible_rotation() {
        float delta_degrees = rotated_bone.parent.delta_degrees(rotated_bone);
        if (delta_degrees > possible_span.max) {
            rotated_bone.rotation = rotated_bone.parent.rotation * degrees_to_quaternion(possible_span.max);
        }
        else if (delta_degrees < possible_span.min) {
            rotated_bone.rotation = rotated_bone.parent.rotation * degrees_to_quaternion(possible_span.min);
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