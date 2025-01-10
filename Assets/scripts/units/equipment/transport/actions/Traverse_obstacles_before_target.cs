#define RVI_DEBUG

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


    public GraphNode find_closest_unobstructed_waypoint(Vector3 start) {
        GraphNode closest_visible_node = null;
        float closest_distance = float.PositiveInfinity;

        foreach (var waypoint in AstarPath.active.data.pointGraph.nodes) {
            var this_distance = ((Vector3) waypoint.position - start).sqrMagnitude;
            if (closest_distance > this_distance) {
                if (
                    !is_waypoint_obstructed((Vector3)waypoint.position,moved_body.position)
                ) {
                    closest_visible_node = waypoint;
                    closest_distance = this_distance;
                }
            }
        }

        return closest_visible_node;
    }

    private GraphNode start_of_path;
    protected override void on_start_execution() {
        base.on_start_execution();

        start_of_path = find_closest_unobstructed_waypoint(moved_body.position);
        if (start_of_path != null) {
            path_seeker.StartPath((Vector3) start_of_path.position, final_target.position, on_patch_found);
        }
        else {
            #if RVI_DEBUG
            Debug.Log($"COMPUTER {moved_body.name} #{transporter.actor.action_runner.number} Traverse_obstacles_before_target::on_start_execution start_of_path==null");
            #endif
        }
    }

    

    private void on_patch_found(Path found_path) {
        if (found_path.error) {
            Debug.Log($"COMPUTER {found_path.errorLog}; when moving from {moved_body} to {final_target}");
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
            !Keep_distance_from_target.is_target_obstructed_by_walls(final_target,moved_body.position)
            ||
            start_of_path == null
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
            (!is_waypoint_obstructed((Vector3) path.path[i_waypoint].position, moved_body.position))
        )
        {
            i_waypoint++;
        }
        return i_waypoint - 1;
    }


    public static bool is_waypoint_obstructed(Vector3 end, Vector3 start) {
        var vector_to_target =(Vector2)end - (Vector2)start;
        var hit = Physics2D.Raycast(
            start,
            vector_to_target,
            vector_to_target.magnitude,
            Keep_distance_from_target.permanent_obstacles_of_walking
        );

        if (hit.transform) {
            return true;
        }
        return false;
    }
   
    
    protected override void restore_state() {
        base.restore_state();
        path_seeker.pathCallback -= on_patch_found;
    }
    
}
}