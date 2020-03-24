using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rvinowise;
using rvinowise.units.parts.limbs.arms.actions;
using rvinowise.units.parts.strategy;
using Action = rvinowise.units.parts.limbs.arms.actions.Action;


namespace rvinowise.units.parts.limbs.arms.strategy {

public class Take_tool_from_bag: Action {

    private Baggage bag;

    public static Take_tool_from_bag create(Action_tree in_action_tree, Baggage in_bag) {
        Take_tool_from_bag strategy = new Take_tool_from_bag(in_action_tree, in_bag); //todo from pool?
        
        return strategy;
    }
    public Take_tool_from_bag() {
    }
    public Take_tool_from_bag(Action_tree in_action_tree, Baggage in_bag) : base(in_action_tree) {
        bag = in_bag;
    }
    
    public override void update() {
        
    }
}
}