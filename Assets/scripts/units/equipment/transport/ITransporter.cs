using rvinowise.unity.actions;
using UnityEngine;


namespace rvinowise.unity {

public static class Physics_consts {
    public const float rigidbody_impulse_multiplier = 10000f;
}

}

namespace rvinowise.unity {



/* provides information about possible speed and rotation for a moving Unit */

public interface ITransporter {
    float get_possible_rotation();
    float get_possible_impulse();
    void set_moved_body(Turning_element in_body);
    Turning_element get_moved_body();

    void move_towards_destination(Vector2 destination);
    void face_rotation(Quaternion rotation);
    
}


public interface IActor_transporter :
    ITransporter
    , IActor 
{ }

}