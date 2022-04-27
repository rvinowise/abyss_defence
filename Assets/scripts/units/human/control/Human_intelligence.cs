using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rvinowise.unity.extensions;

using rvinowise;
using rvinowise.unity.units.humanoid;
using rvinowise.unity.units.parts;
using rvinowise.unity.units.parts.limbs.arms;


namespace rvinowise.unity.units.control.human {

public class Human_intelligence:Intelligence {

    public Humanoid user;
    public parts.limbs.arms.humanoid.Arm_pair arm_pair;

    public int current_equipped_set;
    protected override void Awake() {
        base.Awake();
        if (arm_pair == null) {
            arm_pair = GetComponent<parts.limbs.arms.humanoid.Arm_pair>();
        }
        user = GetComponent<Humanoid>();
    }

    /*#region IAction 
    public override void on_lacking_action() {
        
    }
    #endregion*/
}
}