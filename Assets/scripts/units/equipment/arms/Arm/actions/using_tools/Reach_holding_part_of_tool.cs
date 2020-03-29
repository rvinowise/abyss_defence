using System;
using System.Collections;
using System.Collections.Generic;
using geometry2d;
using UnityEngine;
using rvinowise;
using rvinowise.units.parts.limbs.arms.actions;
using rvinowise.units.parts.tools;
using Action = rvinowise.units.parts.limbs.arms.actions.Action;


namespace rvinowise.units.parts.limbs.arms.strategy {

public class Reach_holding_part_of_tool:Action_reach_somewhere {

    private Holding_place holding_place;
    private Tool tool {
        get { return holding_place.tool; }
    }
    
    public static Reach_holding_part_of_tool create(
        Action_tree in_action_tree, Holding_place holding_place
    ) {
        var action = (Reach_holding_part_of_tool)pool.get(typeof(Reach_holding_part_of_tool), in_action_tree);
        action.holding_place = holding_place;
        return action;
    }

    public override void start() {
        arm.hand.gesture = Hand_gesture.Support_of_horizontal;
    }



    protected override Orientation get_desired_orientation() {
        return new Orientation(
            holding_place.position - (Vector2)(holding_place.rotation * arm.hand.tip) ,
            holding_place.rotation
        );
    }
    
    protected override bool complete(Orientation desired_orientation) {
        return base.complete(desired_orientation);
    }
}
}