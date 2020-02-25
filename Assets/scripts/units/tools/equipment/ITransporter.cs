﻿using System;
using System.Collections;
using UnityEngine;

namespace rvinowise.units.equipment {
    
/* provides information about possible speed and rotation for a moving Unit */
public interface ITransporter: IEquipment_controller {
    float get_possible_rotation();
    float get_possible_impulse();
    //new ITransporter copy_empty_into(User_of_equipment dst_host);
    void rotate_to_direction(float direction);
    void move_in_direction(float direction);
    void move_in_direction(Vector2 direction);
}

}