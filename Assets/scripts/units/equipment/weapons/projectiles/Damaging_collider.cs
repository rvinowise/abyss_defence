using System.Collections.Generic;
using System.Linq;
using rvinowise.unity.effects.trails.mesh_impl;
using rvinowise.unity.geometry2d;
using UnityEngine;
using rvinowise.unity.extensions;
using UnityEngine.Serialization;


namespace rvinowise.unity {

[RequireComponent(typeof(Rigidbody2D))]
public class Damaging_collider : MonoBehaviour {

    public Damage_dealer damage_dealer;
    
    public Rigidbody2D rigid_body;
    public Collider2D collider2d;
    public Explosive_body[] explosive_bodies;
    
    /* OnCollision callback fires after the velocity changes. The relevant velocity should be stored here  */
    public readonly Saved_physics last_physics = new Saved_physics();
    public void store_last_physics() {
        last_physics.velocity = rigid_body.velocity;
        last_physics.position = transform.position;
    }

    void OnEnable() {
        store_last_physics();
    }

    
    void Awake() {
        rigid_body = GetComponent<Rigidbody2D>();
        collider2d = GetComponent<Collider2D>();
        damage_dealer = GetComponent<Damage_dealer>();
        explosive_bodies = GetComponents<Explosive_body>();
    }
    
    
    
    void FixedUpdate() {
        store_last_physics();
    }
    
    void Update() {
        if (
            (has_left_map())
        ) {
            Destroy(gameObject);
        }
    }

    private bool has_left_map() {
        return !Map.instance.has(this.transform);
    }
    
    
    
    void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.GetComponent<Damage_receiver>() is {} damage_receiver) {
            if (damage_dealer.is_ignoring_damage_receiver(damage_receiver)) {
                return;
            }
        }
        if (collision.gameObject.GetComponent<IBleeding_body>() is null) {
            var hit = collision.contacts.First();
            damage_dealer.create_hit_impact(hit.point, hit.normal);
        }
        
        if (!GetComponent<Collider2D>().isActiveAndEnabled) {
            //at high speeds, projectile mistakenly bounses off the target, even though it should stop at the target.
            //switching its collider off at the first collision signifies that it shouldn't bounce and collide anymore
            return; 
        }
        Debug.Log($"AIMING: ({name})Damaging_collider.OnCollisionEnter2D()");
        foreach(var explosive_body in explosive_bodies) {
            explosive_body.on_start_dying();
        }
            
    }
    
    
    
}


}