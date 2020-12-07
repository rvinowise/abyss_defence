using System.Collections;
using System.Collections.Generic;

using rvinowise.unity.geometry2d;
using rvinowise.unity.units;
using UnityEngine;
using rvinowise.unity.extensions;
//using rvinowise.unity.effects.trails.line_renderer_impl;
using rvinowise.unity.maps;
using rvinowise.unity.debug;
using rvinowise.unity.effects.physics;
using rvinowise.unity.effects.persistent_residue;
using rvinowise.unity.effects.trails.mesh_impl;

namespace rvinowise.unity.units.parts.weapons.guns.common {

[RequireComponent(typeof(Rigidbody2D))]
//[RequireComponent(typeof(Saved_physics))]
public abstract class Projectile : MonoBehaviour {

    [HideInInspector]
    public Rigidbody2D rigid_body;

    /* OnCollision callback fires after the velocity changes. The relevant velocity should be stored here  */
    public Saved_physics last_physics = new Saved_physics();

    public Smoke_trail trail;
    
    private Trajectory_flyer trajectory_flyer;
    private Collider2D collider;
    

    void Awake() {
        init_components();
    }

    void OnEnable() {
        init_instance();
        init_trail();
        store_last_physics();
    }

    private void init_components() {
        rigid_body = GetComponent<Rigidbody2D>();
        trajectory_flyer = GetComponent<Trajectory_flyer>();
        trajectory_flyer.enabled = false;
        collider = GetComponent<Collider2D>();
        //trail = GetComponent<Smoke_trail>();
        
    }

    private void init_instance() {
        collider.enabled = true;
    }
    private void init_trail() {
        if (trail != null) {
            trail.init_first_points(
                transform.position,
                transform.rotation.to_vector()
            );
            trail.enabled = true;
        }
    }

    void Start() {
        
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
        trajectory_flyer.height = 0f;
        trajectory_flyer.enabled = false;
        collider.enabled = false;
        on_fall_on_ground();
        trail.visit_final_point(transform.position);
        trail.adjust_texture_at_end();
    }
    public void stop_at_position(Vector2 in_point) {
        transform.position = in_point;
        stop_on_the_ground();
    }

    private bool is_on_the_ground() {
        return trajectory_flyer.is_on_the_ground();
    }

    private bool has_left_map() {
        return !Map.instance.has(this.transform);
    }

 

    

    public void store_last_physics() {
        last_physics.velocity = rigid_body.velocity;
        last_physics.position = transform.position;
    }
    
    private void OnCollisionEnter2D(Collision2D collision) {
        if (!collider.enabled) {
            return;
        }
        Rigidbody2D other_creature = collision.gameObject.GetComponent<Rigidbody2D>();
        
        //debug_draw_collision(collision);
        Vector2 contact_point = collision.GetContact(0).point;
        Vector2 new_direction = collision.otherRigidbody.velocity.normalized;
        trail.add_bend_at(
            contact_point,
            new_direction
            //Vector2.one
        );
        
        
        /* UnityEngine.Debug.DrawLine(
            contact_point, contact_point+collision.otherRigidbody.velocity.normalized, 
            Color.magenta, 5); */
        if (new_direction.is_normalized()) {
            UnityEngine.Debug.DrawLine(
                contact_point, contact_point+collision.GetContact(0).relativeVelocity.normalized, 
                Color.yellow, 5);
        } else {
            UnityEngine.Debug.DrawLine(
                contact_point, contact_point+Vector2.up/2, 
                Color.red, 5);
        }

        //after_bouncing_off_surfice(); #test
    }

    private void after_bouncing_off_surfice() {
        trajectory_flyer.enabled = true;
        trajectory_flyer.vertical_velocity = 1f + Random.value*3;
    }

    /* called by Trajectory_flyer.on_fell_on_the_ground() */
    public void on_fall_on_ground() {
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

    public void destroy() {
        gameObject.destroy();
    }

    private bool can_be_deleted() {
        return 
            trajectory_flyer.is_on_the_ground() && 
            !trail.has_visible_parts();
    }

    private void end_active_life() {
        GetComponent<Leaving_persistent_sprite_residue>().leave_persistent_image();
        destroy();
    }

    private void debug_draw_collision(Collision2D other) {
        Vector2 contact_point = other.GetContact(0).point;
        Ray2D ray_of_impact = new Ray2D(
            contact_point, other.GetContact(0).relativeVelocity
        );
        
        Debug_drawer.instance.draw_collision(ray_of_impact, 2f);
    }


    public abstract Polygon get_damaged_area(Ray2D in_ray);


    
}


}