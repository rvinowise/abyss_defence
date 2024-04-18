using rvinowise.unity.geometry2d;
using rvinowise.unity;
using UnityEngine;


namespace rvinowise.unity.actions {

public class Turning_element_reach_direction: Action_leaf {
    private Turning_element_actor turning_element;

    Degree target_direction;
    
    public static Action create(
        Turning_element_actor turning_element, 
        Degree target_direction
    ) {
        
        var action = (Turning_element_reach_direction)object_pool.get(typeof(Turning_element_reach_direction));
        
        action.add_actor(turning_element);
        action.turning_element = turning_element;
        
        action.target_direction = target_direction;
        
        return action;
    }

    protected override void on_start_execution() {
        base.on_start_execution();
        turning_element.target_degree = target_direction;
    }

    public override void update() {

        if (complete()) {
            mark_as_completed();
        } else {
            turning_element.rotate_to_desired_direction();
        }
    }


    private bool complete() {
        return turning_element.at_desired_rotation();
    }

}
}