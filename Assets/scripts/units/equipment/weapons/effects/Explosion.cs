using System;
using System.Collections.Generic;
using rvinowise.unity.geometry2d;
using UnityEngine;
using UnityEngine.Serialization;


namespace rvinowise.unity {

[RequireComponent(typeof(Damage_dealer))]
public class Explosion: MonoBehaviour {

    public List<ParticleSystem> particle_systems = new List<ParticleSystem>();

    public float current_radius;
    public float max_radius = 2f;
    public float push_power = 5f;
    public float dent_depth = 0.5f;
    private float radius_growth;
    private float longest_particle_system_lifetime;

    public CircleCollider2D collider;

    private Polygon shock_wave_polygon_debug;
    private ISet<Transform> hit_targets = new HashSet<Transform>();

    private void Awake() {
        damage_dealer = GetComponent<Damage_dealer>();
        collider = GetComponent<CircleCollider2D>();
        
        foreach (var particle_system in particle_systems) {
            if (longest_particle_system_lifetime < particle_system.main.duration) {
                longest_particle_system_lifetime = particle_system.main.duration;
            }
        }

        radius_growth = max_radius / longest_particle_system_lifetime;
    }

    void Start() {
        
        Destroy(gameObject,longest_particle_system_lifetime);
    }

    private void OnDestroy() {
        damage_dealer.forget_damaged_targets();
    }


    private void FixedUpdate() {
        current_radius += radius_growth*Time.deltaTime;
        collider.radius = current_radius;
        shock_wave_polygon_debug = Polygon_creator.get_circle_polygon(current_radius,10).get_moved(transform.position);
    }

    

    private Damage_dealer damage_dealer;
    public void damage_target(RaycastHit2D target_hit) {
        var target_divisible_body = target_hit.transform.GetComponent<Divisible_body>();
        
        if (target_divisible_body != null) {
            var impact_vector =
                (target_hit.point - (Vector2) transform.position);

            var distance_to_target = impact_vector.magnitude;

            var removed_polygon =
                Polygon_creator.get_circle_polygon(distance_to_target + dent_depth, 10)
                    .move(transform.position);

            Debug_drawer.instance.draw_polygon_debug(removed_polygon);

            target_divisible_body.damage_by_impact(
                removed_polygon,
                target_hit.point,
                impact_vector * push_power
            );
            hit_targets.Add(target_hit.transform);
            damage_dealer.remember_damaged_target(target_hit.transform);
        }
    }

    private bool is_target_was_damaged(Transform target) {
        return hit_targets.Contains(target);
    }

    private bool is_target_unobstructed(Transform target) {
        var hit = get_explosion_collision_towards_target(target);
        if (hit.transform == target) {
            return true;
        }
        return false;
    }

    private RaycastHit2D get_explosion_collision_towards_target(Transform target) {
        var vector_to_target =
            (target.position - transform.position);
        Physics2D.queriesHitTriggers = false;
        
        var hit = Physics2D.Raycast(
            transform.position,
            vector_to_target,
            vector_to_target.magnitude
        );
        
        Physics2D.queriesHitTriggers = true;
        return hit;
    } 
    
    private void OnTriggerEnter2D(Collider2D other) {
        var potential_target = other.transform;
        var hit = get_explosion_collision_towards_target(potential_target);
        if (
            (hit.transform == potential_target)
            &&
            (!damage_dealer.was_target_damaged(potential_target))
        )
        {
            damage_target(hit);
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos() {
        Gizmos.color = Color.green;
        Debug_drawer.draw_polygon_gizmos(shock_wave_polygon_debug);
        Gizmos.color = Color.yellow;
        var max_radius_polygon_debug = Polygon_creator.get_circle_polygon(max_radius, 10).move(transform.position);
            
        Debug_drawer.draw_polygon_gizmos(max_radius_polygon_debug);
    }
#endif
    
}

}