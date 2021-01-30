using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rvinowise.unity.extensions;

using rvinowise;
using rvinowise.unity.units.parts;
using rvinowise.unity.units.parts.limbs.arms;


namespace rvinowise.unity.units.control.human {

public class Human:Intelligence {
    
    
    public parts.limbs.arms.humanoid.Arm_controller arm_controller;

    void Awake() {
        if (arm_controller == null) {
            arm_controller = GetComponent<parts.limbs.arms.humanoid.Arm_controller>();
        }
    }
    
}
}