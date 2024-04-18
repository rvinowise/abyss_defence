using rvinowise.unity.geometry2d;
using rvinowise.unity;
using rvinowise.unity.extensions;
using UnityEngine;


namespace rvinowise.unity.actions {

public class Leg2_touch_target: Limb2_touch_target {
    protected Leg2 leg;

    
    public static Leg2_touch_target Leg2_touch_target_create(
        Leg2 in_leg, 
        Transform in_target
    ) {
        
        var action = object_pool.get<Leg2_touch_target>();
        
        action.add_actor(in_leg);
        action.limb = in_leg;
        action.leg = in_leg;
        action.target = in_target;
        
        return action;
    }

    protected override void on_start_execution() {
        base.on_start_execution();
        leg.raise_up();
    }


}
}