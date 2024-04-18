using System;
using rvinowise.unity.extensions;
using rvinowise.unity.geometry2d;
using UnityEngine;


namespace rvinowise.unity.actions {

public class Scorpion_arm_prepare_jab:Action_leaf {

    private Transform target;
    private Scorpion_pedipalp pedipalp;

    public static Action create(
        Scorpion_pedipalp pedipalp,
        Transform target
    ) {
        var action = (Scorpion_arm_prepare_jab) object_pool.get(typeof(Scorpion_arm_prepare_jab));
        action.add_actor(pedipalp);
        action.pedipalp = pedipalp;
        action.target = target;
            
        return action;
    }

    protected override void on_start_execution() {
        //pedipalp.chila.on_opened_listeners += mark_as_completed;
        pedipalp.chila.animator.SetTrigger("open");
        
        base.on_start_execution();
    }

    
    public override void update() {
        var rotation_to_target = 
            (target.position - pedipalp.chila.transform.position).to_quaternion();

        pedipalp.chila.set_target_rotation(rotation_to_target);
        pedipalp.femur.set_target_rotation(
            rotation_to_target
            * new Degree(170).adjust_to_side(pedipalp.folding_side).to_quaternion()
        );
        
        pedipalp.rotate_to_desired_directions();
        if (
            (pedipalp.chila.pincers_state == Pincers_state.opened)
            &&
            (pedipalp.can_reach(target))
        ) {
            mark_as_completed();
        }
    }

}

}