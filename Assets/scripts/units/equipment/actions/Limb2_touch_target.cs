using rvinowise.unity.geometry2d;
using rvinowise.unity;
using rvinowise.unity.extensions;
using UnityEngine;


namespace rvinowise.unity.actions {

public class Empty_action: Action_leaf {

    
    public static Empty_action create(
    ) {
        
        var action = object_pool.get<Empty_action>();
        
        return action;
    }


    public override void update() {
        mark_as_completed();
    }

}
}