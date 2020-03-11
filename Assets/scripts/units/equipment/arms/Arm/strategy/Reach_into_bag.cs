using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rvinowise;


namespace rvinowise.units.parts.limbs.arms.strategy {

public class Reach_into_bag: arms.strategy.Strategy {

    private Transform bag;

    public Reach_into_bag(Transform in_bag) : base() {
        bag = in_bag;
    }
    
    public override void set_desired_directions(Arm arm) {
        
    }
}
}