using UnityEngine;
using rvinowise.unity.extensions;
using rvinowise.unity;


namespace rvinowise.unity.actions {

public class Keep_distance_from_target: Action_leaf {

    private IActor_transporter transporter;
    private Transform target;
    private Transform moved_body;
    private float optimal_distance = 1;
    
    public static Keep_distance_from_target create(
        IActor_transporter in_transporter,
        Transform moved_body,
        float optimal_distance,
        Transform in_target
    ) {
        var action = (Keep_distance_from_target)object_pool.get(typeof(Keep_distance_from_target));
        action.add_actor(in_transporter);
        
        action.target = in_target;
        action.transporter = in_transporter;
        action.moved_body = moved_body;
        action.optimal_distance = optimal_distance;
        return action;
    }
    public Keep_distance_from_target() {
        
    }


    public override void update() {
        base.update();

        if (target != null) {
            float distance_to_target = (target.position - moved_body.position).magnitude;
            Vector2 vector_to_target =
                (target.position - moved_body.position).normalized;
            if (distance_to_target > optimal_distance) {
                transporter.move_towards_destination(target.position);
            }
            else {
                transporter.move_towards_destination(moved_body.position - target.position);
            }

            transporter.face_rotation(moved_body.quaternion_to(target.position));
        }
        else {
            mark_as_completed();
        }
    }

    private bool has_reached_target() {
        return moved_body.distance_to(target.position) < 1;
    }
   
}
}