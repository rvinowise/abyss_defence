using System.Linq;
using Pathfinding;
using UnityEngine;
using rvinowise.unity.extensions;
using rvinowise.unity;


namespace rvinowise.unity.actions {

public class Follow_target: Action_sequential_parent {

    private ITransporter transporter;
    private Transform moved_body;
    private Seeker path_seeker;
    private Transform final_target;
    private float optimal_distance;
    
    private Path path;
    private int current_waypoint;
    
    public static Follow_target create(
        ITransporter in_transporter,
        Transform moved_body,
        Seeker in_seeker,
        Transform in_target,
        float optimal_distance
    ) {
        var action = (Follow_target)object_pool.get(typeof(Follow_target));
        
        action.final_target = in_target;
        action.transporter = in_transporter;
        action.moved_body = moved_body;
        action.path_seeker = in_seeker;
        action.optimal_distance = optimal_distance;
        return action;
    }
    public Follow_target() {
        
    }


    protected override void on_start_execution() {
        base.on_start_execution();

        add_children();
    }

    private void add_children() {
        if (Keep_distance_from_target.is_target_obstructed_by_walls(final_target,moved_body)) {
            add_child(
                Traverse_obstacles_before_target.create(
                    transporter,
                    moved_body,
                    path_seeker,
                    final_target
                )
            );
        }
        add_child(
            Keep_distance_from_target.create(
                transporter,
                moved_body,
                optimal_distance,
                final_target
            )
        );
    }

    protected override void restore_state() {
        base.restore_state();
    }

    public override void on_child_completed(Action sender_child) {
        if (!queued_child_actions.Any()) {
            add_children();
        }
        base.on_child_completed(sender_child);
        
    }

    public override void update() {
        base.update();
        if (moved_body.gameObject.name == "test") {
            bool test = true;
        }
    }


    
   
}
}