using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using rvinowise.unity.extensions;
using rvinowise.unity.geometry2d;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;


namespace rvinowise.unity {

[RequireComponent(typeof(Damage_dealer))]
public class Explosion: MonoBehaviour {

    public List<ParticleSystem> particle_systems = new List<ParticleSystem>();

    public float current_radius;
    public float max_radius = 2f;
    public float time_to_reach_max_radius = 0.5f;
    public float push_power = 5f;
    public float dent_depth = 0.5f;
    public GameObject ground_mark;
    private float radius_growth;
    private float longest_particle_system_lifetime;

    [FormerlySerializedAs("collider")] public CircleCollider2D collider2d;

    private Polygon shock_wave_polygon_debug;

    private Damage_dealer damage_dealer;
    
    private void Awake() {
        damage_dealer = GetComponent<Damage_dealer>();
        collider2d = GetComponent<CircleCollider2D>();
        
        if (!particle_systems.Any()) {
            particle_systems = GetComponentsInChildren<ParticleSystem>().ToList();
        }
        
        foreach (var particle_system in particle_systems) {
            if (longest_particle_system_lifetime < particle_system.main.duration) {
                longest_particle_system_lifetime = particle_system.main.duration;
            }
        }

        radius_growth = max_radius / time_to_reach_max_radius;
    }

    void Start() {
        leave_ground_mark();
        
        Invoke(nameof(on_radius_reached_maximum),time_to_reach_max_radius);
        Destroy(gameObject,longest_particle_system_lifetime);
    }

    void leave_ground_mark() {
        if (ground_mark != null) {
            ground_mark.transform.SetParent(null,true);
            ground_mark.transform.set_z(Map.instance.ground_z);
            ground_mark.transform.rotation = new Degree(Random.Range(0, 360)).to_quaternion();
            ground_mark.SetActive(true);
        }
    }


    private void on_radius_reached_maximum() {
        radius_growth = 0;
    }
    private void OnDestroy() {
        damage_dealer.forget_damaged_targets();
    }

    public float get_global_scale() {
        Contract.Assert(Mathf.Approximately(transform.lossyScale.x, transform.lossyScale.y), "scale of an explosion should be round");
        return transform.lossyScale.x;
    }
    private void FixedUpdate() {
        current_radius += radius_growth*Time.deltaTime;
        collider2d.radius = current_radius;
        shock_wave_polygon_debug = 
            Polygon_creator.get_circle_polygon(
                current_radius*get_global_scale(),
                10
                ).get_moved(transform.position);
    }
    
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
            damage_dealer.remember_damaged_target(target_hit.transform);
        }

        var target_damage_receiver = target_hit.transform.GetComponent<Damage_receiver>();
        if (target_damage_receiver != null) {
            target_damage_receiver.receive_damage(damage_dealer.effect_amount);
            damage_dealer.remember_damaged_target(target_hit.transform);
        }
        
        if (
            (target_hit.transform.GetComponent<Projectile>() is null)
            &&
            (target_hit.transform.GetComponent<Rigidbody2D>() is {} rigid_body&& rigid_body != null) 
        )
        {
            rigid_body.AddForce(
                calculate_push_vector(
                    rigid_body.transform.position,
                    target_hit.point
                ),
                ForceMode2D.Impulse
            );
        }
    }

    private Vector2 calculate_push_vector(Vector2 target_position, Vector2 hit_point) {
        var impact_vector = (target_position - (Vector2) transform.position);

        var weakening_with_distance =
            1 - impact_vector.magnitude / (max_radius*get_global_scale());
            
        return impact_vector.normalized * push_power * weakening_with_distance;
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
        
        var hit = Finding_objects.raycast(
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
        var max_radius_polygon_debug = 
            Polygon_creator.get_circle_polygon(
                max_radius*get_global_scale(),
                10).move(transform.position);
            
        Debug_drawer.draw_polygon_gizmos(max_radius_polygon_debug);
    }
#endif
    
}

}