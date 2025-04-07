using System.Linq;
using rvinowise.unity.effects.trails.mesh_impl;
using rvinowise.unity.geometry2d;
using UnityEngine;
using rvinowise.unity.extensions;
using UnityEngine.Serialization;


namespace rvinowise.unity {

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour {

    public Smoke_trail_emitter_static_tip trail_emitter;
    public Damage_dealer damage_dealer;
    
    public Rigidbody2D rigid_body;
    public Collider2D collider2d; 
    
    
    /* OnCollision callback fires after the velocity changes. The relevant velocity should be stored here  */
    public readonly Saved_physics last_physics = new Saved_physics();
    public void store_last_physics() {
        last_physics.velocity = rigid_body.velocity;
        last_physics.position = transform.position;
    }

    public Polygon get_damaged_area(Ray2D in_ray) {
        return Damaging_polygons.get_splitting_wedge(in_ray);
        //return Damaging_polygons.get_damaging_circle(in_ray);
    }

    
    void Awake() {
        rigid_body = GetComponent<Rigidbody2D>();
        
        collider2d = GetComponent<Collider2D>();
        damage_dealer = GetComponent<Damage_dealer>();
    }
    
    void OnEnable() {
        store_last_physics();
    }
    
    public void on_restored_from_pool() {
        collider2d.enabled = true;
    }
    
    
    void FixedUpdate() {
        store_last_physics();
    }
    
    void Update() {
        if (
            (!is_on_the_ground())&&
            (has_left_map())
        ) {
            stop_on_the_ground();
        }
    }

    private void stop_on_the_ground() {
        
        collider2d.enabled = false;
        fall_on_ground();
        if (trail_emitter.is_active()) {
            trail_emitter.visit_final_point(transform.position);
        }
    }
    public void stop_at_position(Vector2 in_point) {
        transform.position = in_point;
        stop_on_the_ground();
    }
    
    private bool is_on_the_ground() {
        return !collider2d.enabled;
    }

    private bool has_left_map() {
        return !Map.instance.has(this.transform);
    }
    
    /* called by Trajectory_flyer.on_fell_on_the_ground() */
    public void fall_on_ground() {
        rigid_body.velocity = Vector3.zero;
        rigid_body.angularVelocity = 0;
        if(can_be_deleted()) {
            end_active_life();
        }
    }

    /* called by Trail.disappear() */
    public void on_trail_disappeared() {
        if(can_be_deleted()) {
            end_active_life();
        }
    }

    private bool can_be_deleted() {
        return 
            is_on_the_ground() && 
            !trail_emitter.has_visible_parts();
    }

    private void end_active_life() {
        GetComponent<ILeaving_persistent_residue>()?.leave_persistent_residue();
    }

    private void debug_draw_collision(Collision2D other) {
        Vector2 contact_point = other.GetContact(0).point;
        Ray2D ray_of_impact = new Ray2D(
            contact_point, other.GetContact(0).relativeVelocity
        );
        
        Debug_drawer.instance.draw_collision(ray_of_impact, 2f);
    }
    
    void OnCollisionEnter2D(Collision2D collision) {
        Debug.Log($"AIMING: Projectile.OnCollisionEnter2D victim={collision.gameObject}, projectile={gameObject}");
        if (collision.gameObject.GetComponent<Damage_receiver>() is {} damage_receiver) {
            if (damage_dealer.is_ignoring_damage_receiver(damage_receiver)) {
                return;
            }
            damage_receiver.receive_damage(damage_dealer.effect_amount);
        }
        if (collision.gameObject.GetComponent<IBleeding_body>() is null) {
            var hit = collision.contacts.First();
            damage_dealer.create_hit_impact(
                hit.point.with_height(transform.position.z), 
                hit.normal
            );
        }
        
        if (!GetComponent<Collider2D>().isActiveAndEnabled) {
            //at high speeds, projectile mistakenly bounses off the target, even though it should stop at the target.
            //switching its collider off at the first collision signifies that it shouldn't bounce and collide anymore
            return; 
        }
        
        stop_at_position(collision.GetContact(0).point);
            
    }
    
    
    
}


}