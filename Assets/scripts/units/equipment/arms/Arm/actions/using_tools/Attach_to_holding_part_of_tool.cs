using System.Linq.Expressions;
using geometry2d;
using rvinowise.units.parts.actions;
using UnityEngine;
using rvinowise.units.parts.tools;


namespace rvinowise.units.parts.limbs.arms.actions {

public class Attach_to_holding_part_of_tool: Action_of_arm {

    private Holding_place holding_place;
    
    public static Attach_to_holding_part_of_tool create(
        Action_sequential_parent in_action_sequential_parent, 
        Holding_place holding_place
    ) {
        var action = (Attach_to_holding_part_of_tool)pool.get(typeof(Attach_to_holding_part_of_tool), in_action_sequential_parent);
        action.holding_place = holding_place;
        return action;
    }

    public override void init_state() {
        base.init_state();
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