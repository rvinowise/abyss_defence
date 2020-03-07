using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rvinowise;
using rvinowise.units.parts.limbs;


namespace rvinowise.units.parts.limbs.arms.humanoid {

public class Arm_controller: limbs.arms.Arm_controller {

    public Arm left_arm {
        get { return arms[0]; }
        set { arms[0] = value; }
    }
    
    public Arm right_arm {
        get { return arms[1]; }
        set { arms[1] = value; }
    }

    public Arm_controller(User_of_equipment user) : base(user) { }
}
}