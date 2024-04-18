using UnityEngine;
using rvinowise.unity.extensions;
using rvinowise.unity;


namespace rvinowise.unity.actions {

public class Move_towards_target: Action_leaf {

    private IActor_transporter transporter;
    private Transform target;
    private float needed_distance;
    private Transform moved_transform;
    
    public static Move_towards_target create(
        IActor_transporter in_transporter,
        float needed_distance,
        Transform in_target
    ) {
        var action = (Move_towards_target)object_pool.get(typeof(Move_towards_target));
        
        action.target = in_target;
        action.transporter = in_transporter;
        action.needed_distance = needed_distance;
        action.add_actor(in_transporter);
        return action;
    }
    public Move_towards_target() {
        
    }

    protected override void on_start_execution() {
        base.on_start_execution();

        this.moved_transform = transporter.get_moved_body().transform;
    }


    public override void update() {
        base.update();
        
        transporter.face_rotation(moved_transform.quaternion_to(target.position));

        transporter.move_towards_destination(target.position);
        
        if (has_reached_target()) {
            mark_as_completed();
        } else {
            mark_as_not_completed();
        }
    }

    private bool has_reached_target() {
        return moved_transform.distance_to(target.position) < needed_distance;
    }
   
}
}