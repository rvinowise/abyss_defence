using rvinowise.unity.geometry2d;
using UnityEngine;
using rvinowise.unity;


namespace rvinowise.unity.actions {

public class Arm_reach_holding_part_of_tool:Arm_reach_somewhere {

    private Holding_place holding_place;
    private Tool tool {
        get { return holding_place.tool; }
    }
    
    public static Arm_reach_holding_part_of_tool create(
        Holding_place holding_place
    ) {
        var action = (Arm_reach_holding_part_of_tool)object_pool.get(typeof(Arm_reach_holding_part_of_tool));
        action.holding_place = holding_place;
        return action;
    }

    protected override void on_start_execution() {
        base.on_start_execution();
        arm.hand.gesture = Hand_gesture.Support_of_horizontal;
    }



    protected override Orientation get_desired_orientation() {
        return new Orientation(
            holding_place.position - (Vector2)(holding_place.rotation * arm.hand.tip) ,
            holding_place.rotation
        );
    }
    
}
}