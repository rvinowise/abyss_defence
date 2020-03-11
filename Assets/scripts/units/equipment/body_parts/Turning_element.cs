using rvinowise.units;
using UnityEngine;
using geometry2d;
using static geometry2d.Directions;

namespace rvinowise.units.parts.limbs{

public class Turning_element: Game_object {
    
    
    public Span possible_span;
    public float rotation_speed;

    public Quaternion desired_direction {
        get { return target_direction.direction;}
        set { target_direction.direction = value; }
    }
    public Relative_direction target_direction;
    
    public Turning_element():base() {
        
    }
    public Turning_element(string name):base(name) {
    }

    public virtual void update() {
        transform.rotate_to(target_direction, rotation_speed);
        preserve_possible_rotation();
    }

    private void check_possible_angles() {
        
    }


    private void preserve_possible_rotation() {
        float delta_degrees = host.delta_degrees(transform);
        if (delta_degrees > possible_span.max) {
            rotation = host.rotation * degrees_to_quaternion(possible_span.max);
        }
        else if (delta_degrees < possible_span.min) {
            rotation = host.rotation * degrees_to_quaternion(possible_span.min);
        }
    }

    public void rotate_to(Quaternion direction) {
        transform.rotate_to(direction, rotation_speed );
    }

    
    
}
}