using System.Collections;
using System.Collections.Generic;
using extensions;
using geometry2d;
using rvinowise.units;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class Projectile : MonoBehaviour {

    [HideInInspector]
    public Rigidbody2D rigid_body;

    /* OnCollision callback fires after the velocity changes. The relevant velocity should be stored here  */
    public Saved_physics last_physics = new Saved_physics();
    
    void Awake() {
        init_components();
        store_last_physics();
    }

    private void init_components() {
        rigid_body = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate() {
        store_last_physics();
    }

    public void store_last_physics() {
        last_physics.velocity = rigid_body.velocity;
        last_physics.position = transform.position;
    }
    
    private void OnCollisionEnter(Collision other) {
        Rigidbody2D other_creature = other.gameObject.GetComponent<Rigidbody2D>();
        /*if (other_creature != null) {
            other_creature.divisible_body.split_by_ray(new Ray2D(
                this.gameObject.transform.position,
                this.gameObject.transform.get_direction_vector()
            ));
        }*/
    }


    public abstract Polygon get_damaged_area(Ray2D in_ray);
}
