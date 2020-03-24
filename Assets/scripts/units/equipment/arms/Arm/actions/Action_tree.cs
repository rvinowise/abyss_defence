using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rvinowise;
using rvinowise.units.parts.strategy;


namespace rvinowise.units.parts.limbs.arms.actions {

public class Action_tree: parts.actions.Action_tree {

    public Arm arm;

    public Action_tree(Arm in_arm) : base() {
        arm = in_arm;
    }
    
    
    
}
}