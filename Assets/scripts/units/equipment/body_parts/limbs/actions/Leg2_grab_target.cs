using rvinowise.unity.geometry2d;
using rvinowise.unity;
using rvinowise.unity.extensions;
using UnityEngine;


namespace rvinowise.unity.actions {

public class Leg2_grab_target: Action_sequential_parent {
    protected Leg2 leg;
    protected Transform target;

    
    public static Leg2_grab_target create(
        Leg2 in_leg, 
        Transform in_target
    ) {
        
        var action = object_pool.get<Leg2_grab_target>();
        
        action.leg = in_leg;
        action.target = in_target;
        
        return action;
    }

    protected override void on_start_execution() {
        
        add_children(
            Leg2_touch_target.Leg2_touch_target_create(
                leg,target    
            ),
            Limb2_hold_onto_target.create(
                leg,target    
            ) 
        );
        
        base.on_start_execution();
        leg.raise_up();
    }


}
}