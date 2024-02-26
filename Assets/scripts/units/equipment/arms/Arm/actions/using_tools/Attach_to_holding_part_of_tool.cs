using rvinowise.unity.geometry2d;
using UnityEngine;
using rvinowise.unity;


namespace rvinowise.unity.actions {

public class Attach_to_holding_part_of_tool: Action_of_arm {

    private Holding_place holding_place;
    
    public static Attach_to_holding_part_of_tool create(
        Holding_place holding_place
    ) {
        var action = (Attach_to_holding_part_of_tool)pool.get(typeof(Attach_to_holding_part_of_tool));
        action.holding_place = holding_place;
        return action;
    }

    protected override void on_start_execution() {
        base.on_start_execution();
        arm.hand.gesture = Hand_gesture.Support_of_horizontal;
    }

    
    public override void update() {
        base.update();
        Orientation orientation = get_orientation();
        set_directions(orientation);
    }
    
    private void set_directions(Orientation hand_orientation) {
        arm.hold_onto_point(hand_orientation.position);
        arm.hand.rotation = hand_orientation.rotation;
    }

    private Orientation get_orientation() {
        return new Orientation(
            holding_place.position - (Vector2)(holding_place.rotation * arm.hand.tip) ,
            holding_place.rotation
        );
    }
}
}