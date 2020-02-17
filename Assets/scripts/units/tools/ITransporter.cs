﻿using System;
using System.Collections;
using UnityEngine;

namespace units {
    
/* provides information about possible speed and rotation for a moving Unit */
public interface ITransporter {
    float get_possible_rotation();
    float get_possible_impulse();
}

}