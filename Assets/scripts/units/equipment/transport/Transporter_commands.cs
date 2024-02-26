using UnityEngine;
using rvinowise.unity.extensions;


namespace rvinowise.unity {

public class Transporter_commands {
    public Vector2 moving_direction_vector;

    public Vector2 get_moving_direction_vector() {
        return moving_direction_vector;
    }
    
    public float face_direction_degrees {
        get { return face_direction_quaternion.to_float_degrees(); }
        set { face_direction_quaternion = Quaternion.Euler(0,0,face_direction_degrees);}
    }

    public Quaternion face_direction_quaternion { get; set; }

    private Vector2 target_position;

    public void set_target_position(Vector2 in_position) {
        target_position = in_position;
    }
    public Vector2 get_target_position() {
        return target_position;
    }
}
}