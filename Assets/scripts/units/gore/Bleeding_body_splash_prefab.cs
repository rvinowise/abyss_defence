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
        Vector2 in_impulse,
        Damage_dealer damage_dealer
    ) {
        var contact_with_height =
            in_position.with_height(this.transform.position.z-0.5f);
        var splash = Instantiate(splash_prefab,contact_with_height, in_impulse.to_quaternion());
        var particles = splash.GetComponent<ParticleSystem>();
        var emission_module = particles.emission;
        emission_module.burstCount = (int) (emission_module.burstCount * damage_dealer.effect_amount);
    }
    

    public void OnCollisionEnter2D(Collision2D other) {
        Projectile collided_projectile = other.gameObject.GetComponent<Projectile>();
        if (collided_projectile != null) {

            Vector2 contact_point = other.GetContact(0).point;

            create_splash(
                contact_point,
                other.GetContact(0).relativeVelocity*collided_projectile.GetComponent<Rigidbody2D>().mass,
                collided_projectile.damage_dealer
            );
            
        }
    }
}
}