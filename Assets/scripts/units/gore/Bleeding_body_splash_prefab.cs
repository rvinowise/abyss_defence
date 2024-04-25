using System;
using UnityEngine;
using rvinowise.unity.extensions;
using rvinowise.unity.geometry2d;

using Random = UnityEngine.Random;


namespace rvinowise.unity {

public class Bleeding_body_splash_prefab: MonoBehaviour {

    public GameObject splash_prefab;
    
    public void create_splash(
        Vector2 in_position,
        Vector2 in_impulse
    ) {
        var contact_with_height =
            in_position.with_height(this.transform.position.z-0.5f);
        Instantiate(splash_prefab,contact_with_height, in_impulse.to_quaternion());
    }
    

    public void OnCollisionEnter2D(Collision2D other) {
        Projectile collided_projectile = other.gameObject.GetComponent<Projectile>();
        if (collided_projectile != null) {

            Vector2 contact_point = other.GetContact(0).point;

            create_splash(
                contact_point,
                other.GetContact(0).relativeVelocity*collided_projectile.GetComponent<Rigidbody2D>().mass
            );
            
        }
    }
}
}