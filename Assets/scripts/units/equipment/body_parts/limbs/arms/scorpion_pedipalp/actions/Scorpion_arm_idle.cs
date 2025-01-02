using UnityEngine;


namespace rvinowise.unity.actions {

public class Scorpion_arm_idle:Action_leaf {

    private Scorpion_pedipalp pedipalp;

    public static Action create(
        Scorpion_pedipalp pedipalp
    ) {
        var action = (Scorpion_arm_idle) object_pool.get(typeof(Scorpion_arm_idle));

        action.add_actor(pedipalp.actor);
        action.pedipalp = pedipalp;
            
        return action;
    }

    protected override void on_start_execution() {
        //pedipalp.chila.animator.SetTrigger("idle");
        
        
        base.on_start_execution();
    }
    
    public override void update() {
        base.update();
        //pedipalp.set_desired_directions_by_position(target.position);
        //pedipalp.rotate_to_desired_directions();
    }

}

}