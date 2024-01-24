using UnityEngine;
using rvinowise.unity.extensions;


namespace rvinowise.unity.units.parts.transport {

public class Transporter_commands {
    public Vector2 moving_direction_vector;

    public float face_direction_degrees {
        get { return face_direction_quaternion.to_float_degrees(); }
        set { face_direction_quaternion = Quaternion.Euler(0,0,face_direction_degrees);}
    }

    public Quaternion face_direction_quaternion { get; set; }
}
}