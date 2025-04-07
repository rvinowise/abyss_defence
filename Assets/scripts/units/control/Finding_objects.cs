using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using rvinowise.unity.extensions;
using rvinowise.contracts;
using rvinowise.unity.actions;
using UnityEngine.Serialization;
using Action = rvinowise.unity.actions.Action;


namespace rvinowise.unity {


public static class Finding_objects
{

    public static TComponent find_closest_component<TComponent>(Vector2 position, IEnumerable<TComponent> components) 
        where TComponent: Component {
        float closest_distance = float.PositiveInfinity;
        TComponent closest_component = null;
        foreach (var component in components) {
            if (component == null) continue;
            var this_distance = (position - (Vector2)component.transform.position).sqrMagnitude;
            if (this_distance < closest_distance) {
                closest_distance = this_distance;
                closest_component = component;
            }
        }
        return closest_component;
    }
    
    public static List<Tuple<TComponent,float>> components_sorted_by_distance<TComponent>(
        Vector2 in_position,
        IReadOnlyCollection<TComponent> components
    ) where TComponent: Component 
    {
        List<Tuple<TComponent, float>> components_and_distances = new List<Tuple<TComponent, float>>();
        foreach (var component in components) {
            var distance = component.transform.sqr_distance_to(in_position);
            components_and_distances.Add(new Tuple<TComponent, float>(component,distance));
        }
        components_and_distances.Sort((tuple1,tuple2) => tuple1.Item2.CompareTo(tuple2.Item2) );
        return components_and_distances;
    }
    
    
    public static RaycastHit2D raycast(
        Vector2 origin,
        Vector2 direction
    ) {
        var hit = raycast(
            origin,
            direction,
            Mathf.Infinity,
            Physics2D.DefaultRaycastLayers
        );
        return hit;
    }
    public static RaycastHit2D raycast(
        Vector2 origin,
        Vector2 direction,
        float distance
    ) {
        var hit = raycast(
            origin,
            direction,
            distance,
            Physics2D.DefaultRaycastLayers
        );
        return hit;
    }
    
    public static RaycastHit2D raycast(
        Vector2 origin,
        Vector2 direction,
        float distance,
        int layer_mask
    ) {
        Physics2D.queriesHitTriggers = false;
        Physics2D.queriesStartInColliders = false;
        var hit = Physics2D.Raycast(
            origin,
            direction,
            distance,
            layer_mask
        );
        return hit;
    }

    public static int raycast_all(
        Vector2 origin, Vector2 direction, RaycastHit2D[] results
    ) {
        Physics2D.queriesHitTriggers = false;
        Physics2D.queriesStartInColliders = false;
        
        var filter = new ContactFilter2D().NoFilter();
        
        return Physics2D.Raycast(
            origin, direction, filter, results
        );
    }

    public static RaycastHit2D[] raycast_hits = new RaycastHit2D[1000];
    
    public static bool are_there_obstacles_between_colliders(
        Collider2D origin, Collider2D target, LayerMask obstacles
    ) {
        var filter = new ContactFilter2D();
        filter.layerMask = obstacles;
        filter.useLayerMask = true;
        
        Vector2 vector_to_target = (Vector2)target.transform.position - (Vector2)origin.transform.position;
        
        int hits_amount = Physics2D.Raycast(
            origin.transform.position,
            vector_to_target,
            filter,
            raycast_hits,
            vector_to_target.magnitude
        );
        for(int i_hit=0; i_hit < hits_amount; i_hit++) {
            if (
                (raycast_hits[i_hit].collider != origin) &&
                (raycast_hits[i_hit].collider != target)) {
                return true;
            }
        }
        return false;
    }
    public static bool are_there_obstacles_between_transforms(
        Transform origin, Transform target, LayerMask obstacles
    ) {
        var filter = new ContactFilter2D();
        filter.layerMask = obstacles;
        filter.useLayerMask = true;
        
        Vector2 vector_to_target = (Vector2)target.transform.position - (Vector2)origin.transform.position;
        
        int hits_amount = Physics2D.Raycast(
            origin.transform.position,
            vector_to_target,
            filter,
            raycast_hits,
            vector_to_target.magnitude
        );
        
        Collider2D[] excluded_colliders = new Collider2D[2] {
            origin.GetComponent<Collider2D>(),
            target.GetComponent<Collider2D>()
        };
        
        for(int i_hit=0; i_hit < hits_amount; i_hit++) {
            if (
                (raycast_hits[i_hit].collider != excluded_colliders[0]) &&
                (raycast_hits[i_hit].collider != excluded_colliders[1])) {
                return true;
            }
        }
        return false;
    }
    
    public static bool are_there_obstacles_between_points(
        Vector2 origin, 
        Vector2 target,
        LayerMask obstacles,
        Collider2D[] excluded_colliders
    ) {
        var filter = new ContactFilter2D();
        filter.layerMask = obstacles;
        filter.useLayerMask = true;
        
        Vector2 vector_to_target = target - origin;
        
        int hits_amount = Physics2D.Raycast(
            origin,
            vector_to_target,
            filter,
            raycast_hits,
            vector_to_target.magnitude
        );
        
        for(int i_hit=0; i_hit < hits_amount; i_hit++) {
            var is_excluded = false;
            foreach (var excluded_collider in excluded_colliders) {
                if (raycast_hits[i_hit].collider == excluded_collider) {
                    is_excluded = true;
                    break;
                }
            }
            if (!is_excluded) {
                return true;
            }
        }
        return false;
    }
}
}