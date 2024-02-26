using rvinowise.unity.actions;
using UnityEngine;


namespace rvinowise.unity {

public static class Physics_consts {
    public const float rigidbody_impulse_multiplier = 1f;
}

}

namespace rvinowise.unity {



/* provides information about possible speed and rotation for a moving Unit */

public interface ITransporter:
    IExecute_commands,
    IActor

{
    float possible_rotation { get; set; }
    float possible_impulse { get; set; }
    
    Transporter_commands command_batch { get; }

    // public void set_desired_face_rotation(Quaternion in_rotation);
    // public void set_desired_position(Vector2 in_position);
}

}