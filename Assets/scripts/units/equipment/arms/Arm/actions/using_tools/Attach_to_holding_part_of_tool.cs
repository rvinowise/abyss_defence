using System.Linq.Expressions;
using geometry2d;
using UnityEngine;
using rvinowise.units.parts.limbs.arms.actions;
using rvinowise.units.parts.tools;


namespace rvinowise.units.parts.limbs.arms.strategy {

public class Attach_to_holding_part_of_tool: Action {

    private Holding_place holding_place;
    
    public static Attach_to_holding_part_of_tool create(
        Action_tree in_action_tree, Holding_place holding_place
    ) {
        var action = (Attach_to_holding_part_of_tool)pool.get(typeof(Attach_to_holding_part_of_tool), in_action_tree);
        action.holding_place = holding_place;
        return action;
    }

    public override void start() {
        arm.hand.gesture = Hand_gesture.Support_of_horizontal;
    }

    
    public override void update() {
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