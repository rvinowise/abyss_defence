using UnityEngine;


namespace rvinowise.unity.actions {

public class Scorpion_arm_attack:Action_sequential_parent {

    private Transform target;
    private Scorpion_pedipalp pedipalp;

    public static Action create(
        Scorpion_pedipalp pedipalp,
        Transform target
    ) {
        var action = (Scorpion_arm_attack) object_pool.get(typeof(Scorpion_arm_attack));

        action.pedipalp = pedipalp;
        action.target = target;
            
        return action;
    }

    protected override void on_start_execution() {
        
        add_children(
            Scorpion_arm_prepare_jab.create(
                pedipalp,target
            ),
            Scorpion_arm_throw_jab.create(
                pedipalp,target
            ),
            Scorpion_arm_pinch_target.create(
                pedipalp,target
            )
        );
        
        base.on_start_execution();
    }

}

}