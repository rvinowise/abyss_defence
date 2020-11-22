using System;
using System.Collections;
using System.Collections.Generic;
using rvinowise.unity.geometry2d;
using UnityEngine;
using rvinowise.unity.extensions;

using rvinowise;
using rvinowise.unity.units.parts.actions;
using rvinowise.unity.units.parts.limbs.arms.actions;
using rvinowise.unity.units.parts.tools;


namespace rvinowise.unity.units.parts.limbs.arms.actions {

public class Arm_reach_holding_part_of_tool:Arm_reach_somewhere {

    private Holding_place holding_place;
    private Tool tool {
        get { return holding_place.tool; }
    }
    
    public static Arm_reach_holding_part_of_tool create(
        Holding_place holding_place
    ) {
        var action = (Arm_reach_holding_part_of_tool)pool.get(typeof(Arm_reach_holding_part_of_tool));
        action.holding_place = holding_place;
        return action;
    }

    public override void init_state() {
        base.init_state();
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