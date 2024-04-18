using rvinowise.unity.extensions;
using rvinowise.unity.geometry2d;
using UnityEngine;


namespace rvinowise.unity.actions {

public class Scorpion_arm_pinch_target:Action_leaf {

    protected Transform target;
    protected Scorpion_pedipalp pedipalp;
    
    public static Action create(
        Scorpion_pedipalp pedipalp,
        Transform target
    ) {
        var action = (Scorpion_arm_pinch_target) object_pool.get(typeof(Scorpion_arm_pinch_target));

        action.add_actor(pedipalp);
        action.pedipalp = pedipalp;
        action.target = target;
        
        return action;
    }

    protected override void on_start_execution() {
        base.on_start_execution();
        
        pedipalp.chila.animator.SetTrigger("close");
        
    }

    public override void update() {
        base.update();
        pedipalp.set_desired_directions_by_position(target.position);
        pedipalp.rotate_to_desired_directions();
        if (pedipalp.chila.pincers_state == Pincers_state.closed) {
            mark_as_completed();
        }
    }

   
    
}

}