using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rvinowise;
using rvinowise.units.equipment.limbs;
using rvinowise.units.equipment.limbs.arms;


namespace units.equipment.arms.humanoid {

public class Arm_controller: arms.Arm_controller {

    public Arm left_arm {
        get { return arms[0]; }
        set { arms[0] = value; }
    }
    
    public Arm right_arm {
        get { return arms[1]; }
        set { arms[1] = value; }
    }

}
}