using System.Collections;
using System.Collections.Generic;
using extesions;
using rvinowise.units;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : MonoBehaviour {


    public Rigidbody2D rigid_body;

    public Saved_physics last_physics = new Saved_physics();
    
    void Awake() {
        init_components();
    }

    private void init_components() {
        rigid_body = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
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
}
