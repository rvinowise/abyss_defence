using rvinowise.unity.geometry2d;
using rvinowise.unity;
using rvinowise.unity.extensions;
using UnityEngine;


namespace rvinowise.unity.actions {

public class Limb2_hold_onto_target: Action_leaf {
    protected Limb2 limb;

    protected Transform target;
    
    public static Limb2_hold_onto_target create(
        Limb2 in_limb, 
        Transform in_target
    ) {
        
        var action = (Limb2_hold_onto_target)object_pool.get(typeof(Limb2_hold_onto_target));
        
        action.add_actor(in_limb);
        action.limb = in_limb;
        action.target = in_target;
        
        return action;
    }

    protected override void on_start_execution() {
        base.on_start_execution();
    }

    public override void update() {

        var directions = limb.determine_directions_reaching_point(target.position);
        if (directions.failed) {
            mark_as_completed();
        }
        else {
            limb.hold_onto_point(target.position);
        }
        
    }

}
}