using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rvinowise;


namespace rvinowise.units.parts.limbs.arms.strategy {

public class Take_tool_from_bag: strategy.Strategy {

    private Baggage bag;

    public static Take_tool_from_bag create(Arm in_arm, Baggage in_bag) {
        Take_tool_from_bag strategy = new Take_tool_from_bag(in_arm, in_bag); //todo from pool?
        
        return strategy;
    }
    
    public Take_tool_from_bag(Arm in_arm, Baggage in_bag) : base(in_arm) {
        bag = in_bag;
    }
    public override void update() {
        
    }
}
}