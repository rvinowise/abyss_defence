using rvinowise.units.equipment;
using UnityEngine;

namespace units.equipment.transport {
    
/* provides information about possible speed and rotation for a moving Unit */
public interface ITransporter: IEquipment_controller {
    float get_possible_rotation();
    float get_possible_impulse();

    //void command(transport.Command_batch command_batch);
    
    transport.Command_batch command_batch { get; }

    
}

}