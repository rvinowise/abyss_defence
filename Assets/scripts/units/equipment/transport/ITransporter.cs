using UnityEngine;
using rvinowise.unity.units.parts.actions;


namespace rvinowise.unity.units.parts.transport {
    
/* provides information about possible speed and rotation for a moving Unit */

public interface ITransporter:
    IExecute_commands,
    IActor

{
    float possible_rotation { get; set; }
    float possible_impulse { get; set; }
    
    Quaternion direction_quaternion { get; }

    transport.Transporter_commands command_batch { get; }
    
}

}