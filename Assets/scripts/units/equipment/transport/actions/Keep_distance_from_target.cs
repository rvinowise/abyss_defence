using UnityEngine;
using rvinowise.unity.extensions;
using rvinowise.unity;


namespace rvinowise.unity.actions {

public class Keep_distance_from_target: Action_leaf {

    private ITransporter transporter;
    private Transform target;
    private Transform moved_body;
    private float optimal_distance = 1;
    
    public static Keep_distance_from_target create(
        ITransporter in_transporter,
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

        if (
            (target != null)&&
            (!is_target_obstructed_by_walls(target,moved_body))
        )
        {
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
    
    
    RaycastHit2D[] hits = new RaycastHit2D[2];
    public static LayerMask permanent_obstacles_of_walking = 
        ~LayerMask.GetMask("projectiles")
        & ~LayerMask.GetMask("bodies")
        ;
    
    public static LayerMask obstacles_of_walking = 
            ~LayerMask.GetMask("projectiles")
        ;

    public static int flying_layer = LayerMask.NameToLayer("flying");
    public static bool is_target_obstructed_by_walls(Transform target, Transform body) {
        if (body.gameObject.layer == flying_layer) {
            return false;
        }
        
        var start = (Vector2) body.position;
        Vector2 vector_to_target = (Vector2)target.position - start;
        var hit = Physics2D.Raycast(
            start,
            vector_to_target,
            vector_to_target.magnitude,
            permanent_obstacles_of_walking
        );

        if (hit.transform) {
            return true;
        }
        return false;
    }
    
    public static bool is_target_obstructed_by_something(Transform target, Vector2 start) {
        var vector_to_target = (Vector2)target.position - start;
        var hit = Physics2D.Raycast(
            start,
            vector_to_target,
            vector_to_target.magnitude,
            obstacles_of_walking
        );

        if (hit.transform != target.transform) { //assuming the target is a body itself
            return true;
        }
        return false;
    }
   
}
}