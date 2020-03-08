using rvinowise.units;
using UnityEngine;
using geometry2d;
using static geometry2d.Directions;

namespace rvinowise.units.parts.limbs{

public class Turning_element: Game_object {
    
    
    public Span possible_span;
    public float rotation_speed;
    public Quaternion old_rotation;

    public Turning_element():base() {
        
    }
    public Turning_element(string name):base(name) {
    }

    public void update() {
        //base.update();
        //check_possible_angles();
        //rotate_with_host();
        preserve_possible_rotation();
    }

    private void check_possible_angles() {
        
    }

    /*private void rotate_with_host() {
        if (host is Turning_element turning_element) {
            rotation *= host.rotation  * Quaternion.Inverse(turning_element.old_rotation);
        }
    }*/

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