using UnityEngine;
using rvinowise.unity.extensions;
using rvinowise.unity.units.parts.actions;
using rvinowise.unity.units.parts.transport;


namespace rvinowise.unity.units.parts.limbs.arms.actions {

public class Keep_distance_from_target: Action_leaf {

    private ITransporter transporter;
    private Transform target;
    private Transform transform;
    private float optimal_distance = 1;
    
    public static Keep_distance_from_target create(
        ITransporter in_transporter,
        float optimal_distance,
        Transform in_target
    ) {
        var action = (Keep_distance_from_target)pool.get(typeof(Keep_distance_from_target));
        action.add_actor(in_transporter);
        
        action.target = in_target;
        action.transporter = in_transporter;
        action.optimal_distance = optimal_distance;
        return action;
    }
    public Keep_distance_from_target() {
        
    }

    public override void init_actors() {
        base.init_actors();
        transform = transporter.gameObject.transform;
    }

    public override void update() {
        base.update();
        float distane_to_target = (target.position - transform.position).magnitude;
        Vector2 vector_to_target = transporter.command_batch.moving_direction_vector = 
            (target.position - transform.position).normalized;
        if (distane_to_target < optimal_distance) {
            vector_to_target = -vector_to_target;
        }
        transporter.command_batch.moving_direction_vector = vector_to_target.normalized;
        
        transporter.command_batch.face_direction_quaternion =
            transform.quaternion_to(target.position);
        
        // if (has_reached_target()) {
        //     mark_as_reached_goal();
        // } else {
        //     mark_as_has_not_reached_goal();
        // }
        mark_as_completed();
    }

    private bool has_reached_target() {
        return transform.distance_to(target.position) < 1;
    }
   
}
}