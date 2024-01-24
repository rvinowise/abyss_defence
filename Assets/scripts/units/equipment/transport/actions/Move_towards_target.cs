using UnityEngine;
using rvinowise.unity.extensions;
using rvinowise.unity.units.parts.actions;
using rvinowise.unity.units.parts.transport;


namespace rvinowise.unity.units.parts.limbs.arms.actions {

public class Move_towards_target: Action_leaf {

    private ITransporter transporter;
    private Transform target;
    private Transform transform;
    private float needed_distance;
    
    public static Move_towards_target create(
        ITransporter in_transporter,
        float needed_distance,
        Transform in_target
    ) {
        var action = (Move_towards_target)pool.get(typeof(Move_towards_target));
        
        action.target = in_target;
        action.transporter = in_transporter;
        action.needed_distance = needed_distance;
        //action.actor = in_transporter;
        return action;
    }
    public Move_towards_target() {
        
    }

    public override void init_actors() {
        base.init_actors();
        transform = transporter.gameObject.transform;
    }

    public override void update() {
        base.update();
        
        transporter.command_batch.moving_direction_vector = 
            (target.position - transform.position).normalized;
        
        transporter.command_batch.face_direction_quaternion =
            transform.quaternion_to(target.position);
        
        if (has_reached_target()) {
            mark_as_completed();
        } else {
            mark_as_not_completed();
        }
    }

    private bool has_reached_target() {
        return transform.distance_to(target.position) < needed_distance;
    }
   
}
}