using rvinowise.units.parts;
using UnityEngine;

namespace rvinowise.units.parts.transport {
    
/* provides information about possible speed and rotation for a moving Unit */
public interface ITransporter: IExecute_commands {
    float possible_rotation { get; set; }
    float possible_impulse { get; set; }
    
    Quaternion direction_quaternion { get; }
    

//    void command(transport.Command_batch command_batch);
    transport.Command_batch command_batch { get; }
    
}

}