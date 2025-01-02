using System.Linq;
using Pathfinding;
using UnityEngine;
using rvinowise.unity.extensions;
using rvinowise.unity;


namespace rvinowise.unity.actions {

public class Traverse_obstacles_before_target: Action_leaf {

    private ITransporter transporter;
    private Transform final_target;
    private Transform moved_body;
    private Seeker path_seeker;

    private Path path;
    private int current_waypoint;
    
    public static Traverse_obstacles_before_target create(
        ITransporter in_transporter,
        Transform moved_body,
        Seeker in_seeker,
        Transform in_target
    ) {
        var action = (Traverse_obstacles_before_target)object_pool.get(typeof(Traverse_obstacles_before_target));
        action.add_actor(in_transporter);
        
        action.final_target = in_target;
        action.transporter = in_transporter;
        action.moved_body = moved_body;
        action.path_seeker = in_seeker;
        return action;
    }
    public Traverse_obstacles_before_target() {
        
    }


    protected override void on_start_execution() {
        base.on_start_execution();

        path_seeker.StartPath(moved_body.position, final_target.position, on_patch_found);
    }

    protected override void restore_state() {
        base.restore_state();
        path_seeker.pathCallback -= on_patch_found;
    }

    private void on_patch_found(Path found_path) {
        if (found_path.error) {
            Debug.Log($"TRANSPORT: {found_path.errorLog}; when moving from {moved_body} to {final_target}");
        }
        else {
            path = found_path;
            current_waypoint = 0;
        }
        
    }
    
    public override void update() {
        base.update();

        if (moved_body.gameObject.name == "test") {
            bool test = true;
        }
        
        if (
            (final_target == null) 
            ||
            is_target_unobstructed()
            )
        {
            mark_as_completed();
            return;
        }
        if (path!= null) {
            var target_point_index = find_farthest_unobstructed_waypoint();
            if (target_point_index >= 0) {
                if (is_reached_end_of_path()) {
                    mark_as_completed();
                } else {
                    current_waypoint = target_point_index;
                    var target_point = path.path[target_point_index];
                    transporter.move_towards_destination((Vector3) target_point.position);
                    transporter.face_rotation(moved_body.quaternion_to((Vector3) target_point.position));
                }
            }
            else {
                //even the first waypoint is obstructed, we can't use the generated path
                mark_as_completed();
            }
        }
        
    }

    public bool is_reached_end_of_path() {
        var close_enough_distance = 0.1;
        var current_waypoint_is_last = current_waypoint == path.path.Count - 1;
        if (current_waypoint_is_last) {
            var last_waypoint = path.path.Last();
            if (moved_body.distance_to((Vector3)last_waypoint.position) < close_enough_distance) {
                return true;
            }
        }
        return false;
    }


    public int find_farthest_unobstructed_waypoint() {
        var i_waypoint = current_waypoint;
        while (
            (i_waypoint < path.path.Count)
            &&
            (is_waypoint_unobstructed((Vector3) path.path[i_waypoint].position))
        )
        {
            i_waypoint++;
        }
        return i_waypoint - 1;
    }
    

    private bool is_target_unobstructed() {
        var vector_to_target = final_target.position - moved_body.position;
        var hit = Physics2D.Raycast(
            moved_body.position,
            vector_to_target
        );

        if (hit.transform == final_target.transform) {
            return true;
        }
        return false;
    }
    
    private bool is_waypoint_unobstructed(Vector3 position) {
        //raycast from the destination up to the source, in order to not be stuck in the initial collider of the ray-caster
        var vector_from_target = (Vector2)moved_body.position - (Vector2)position;
        var hit = Physics2D.Raycast(
            position,
            vector_from_target,
            vector_from_target.magnitude
        );

        if (hit.transform == moved_body.transform) {
            return true;
        }
        return false;
    }
   
}
}