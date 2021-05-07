using System.Collections;
using System.Collections.Generic;

using rvinowise.unity.geometry2d;
using rvinowise.unity.units;
using UnityEngine;
using rvinowise.unity.extensions;
using rvinowise.unity.maps;
using rvinowise.unity.debug;
using rvinowise.unity.effects.physics;
using rvinowise.unity.effects.persistent_residue;
using rvinowise.unity.effects.trails.mesh_impl;

namespace rvinowise.unity.units.parts.weapons.guns.common {

[RequireComponent(typeof(Rigidbody2D))]
public abstract class Bouncing_projectile : Projectile {


    

    public Smoke_trail trail;
    
    private Trajectory_flyer trajectory_flyer;
    

    protected virtual void Awake() {
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
        if (trail.is_active()) {
            trail.visit_final_point(transform.position);
            trail.adjust_texture_at_end();
        }
    }
    public override void stop_at_position(Vector2 in_point) {
        transform.position = in_point;
        stop_on_the_ground();
    }

    private bool is_on_the_ground() {
        return trajectory_flyer.is_on_the_ground();
    }

    private bool has_left_map() {
        return !Map.instance.has(this.transform);
    }

 

    

    
    
    private void OnCollisionEnter2D(Collision2D collision) {
        if (!collider.enabled) {
            return;
        }
        Rigidbody2D other_creature = collision.gameObject.GetComponent<Rigidbody2D>();
        
        //debug_draw_collision(collision);
        Vector2 contact_point = collision.GetContact(0).point;
        Vector2 new_direction = collision.otherRigidbody.velocity.normalized;
        if (trail.is_active()) {
            trail.add_bend_at(
                contact_point,
                new_direction
            );
        }
        
        if (new_direction.is_normalized()) {
            UnityEngine.Debug.DrawLine(
                contact_point, contact_point+collision.GetContact(0).relativeVelocity.normalized, 
                Color.yellow, 5);
        } else {
            UnityEngine.Debug.DrawLine(
                contact_point, contact_point+Vector2.up/2, 
                Color.red, 5);
        }
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
        GetComponent<Leaving_persistent_sprite_residue>().leave_persistent_residue();
        destroy();
    }

    private void debug_draw_collision(Collision2D other) {
        Vector2 contact_point = other.GetContact(0).point;
        Ray2D ray_of_impact = new Ray2D(
            contact_point, other.GetContact(0).relativeVelocity
        );
        
        Debug_drawer.instance.draw_collision(ray_of_impact, 2f);
    }




    
}


}