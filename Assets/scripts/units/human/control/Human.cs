using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rvinowise;
using rvinowise.units.parts;
using rvinowise.units.parts.limbs.arms;


namespace rvinowise.units.control.human {

public class Human:Intelligence {
    
    
    public parts.limbs.arms.humanoid.Arm_controller arm_controller;
    
    
    public Human() : base() { }
}
}