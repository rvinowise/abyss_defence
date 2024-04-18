using rvinowise.unity.extensions;
using rvinowise.unity.geometry2d;
using UnityEngine;


namespace rvinowise.unity.actions {

public class Scorpion_arm_throw_jab:Action_leaf {

    protected Transform target;
    protected Scorpion_pedipalp pedipalp;
    
    public static Action create(
        Scorpion_pedipalp pedipalp,
        Transform target
    ) {
        var action = (Scorpion_arm_throw_jab) object_pool.get(typeof(Scorpion_arm_throw_jab));

        action.add_actor(pedipalp);
        action.pedipalp = pedipalp;
        action.target = target;
            
        return action;
    }


    public override void update() {
        base.update();
        pedipalp.set_desired_directions_by_position(target.position);
        pedipalp.rotate_to_desired_directions();
        if (is_chila_close_to_target()) {
            mark_as_completed();
        }
    }

    private bool is_chila_close_to_target() {
        var attacking_radius = 0.5f;

        return
            target.position.distance_to(
                pedipalp.chila.tip
            ) <= attacking_radius;
    }

    
}

}