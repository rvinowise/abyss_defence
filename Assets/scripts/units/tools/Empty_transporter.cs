using System;
using System.Collections;
using UnityEngine;

namespace units {
    
/* provides information about possible speed and rotation for a moving Unit */
public class Empty_transporter: ITransporter {
    public float get_possible_rotation() {
        return 0;
    }

    public float get_possible_impulse() {
        return 0;
    }
}

}