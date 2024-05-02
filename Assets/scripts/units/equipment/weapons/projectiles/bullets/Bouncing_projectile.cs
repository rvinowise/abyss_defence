using UnityEngine;
using rvinowise.unity.extensions;
using rvinowise.unity.effects.trails.mesh_impl;
using UnityEngine.Serialization;


namespace rvinowise.unity {

[RequireComponent(typeof(Rigidbody2D))]
public abstract class Bouncing_projectile : MonoBehaviour {

    public Smoke_trail_emitter_static_tip trail_emitter;
    
    public Trajectory_flyer trajectory_flyer;

    public Rigidbody2D rigid_body;
    public Collider2D collider; 

    protected virtual void Awake() {
        rigid_body = GetComponent<Rigidbody2D>();
        trajectory_flyer = GetComponent<Trajectory_flyer>();
        trajectory_flyer.enabled = false;
        collider = GetComponent<Collider2D>();
    }

    public void on_restored_from_pool() {
        collider.enabled = true;
    }

    
    private void OnCollisionEnter2D(Collision2D collision) {
        if (!collider.enabled) {
            return;
        }
        
        Vector2 contact_point = collision.GetContact(0).point;
        Vector2 new_direction = collision.otherRigidbody.velocity.normalized;
        if (trail_emitter.is_active()) {
            trail_emitter.add_bending_at(
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

    




    
}


}