using System;
using UnityEngine;
using rvinowise.unity.extensions;
using rvinowise.unity.geometry2d;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;


namespace rvinowise.unity {

public class Bleeding_body_splash_prefab: 
    MonoBehaviour,
    IBleeding_body
{

    public GameObject back_splash_prefab;
    public GameObject frontal_splash_prefab;
    
    
    public void create_splash(
        Vector2 in_position,
        Vector2 in_impulse
    ) {
        create_back_splash(
            in_position,
            in_impulse
        );
        create_frontal_splash(
            in_position,
            in_impulse
        );
    }
    
    
    public void create_back_splash(
        Vector2 in_position,
        Vector2 in_impulse
    ) {
        var contact_with_height =
            in_position.with_height(this.transform.position.z-0.5f);
            //in_position.with_height(0);
        var splash = Instantiate(back_splash_prefab,contact_with_height, (in_impulse*+1).to_quaternion());
    }
    
    public void create_frontal_splash(
        Vector2 in_position,
        Vector2 in_impulse
    ) {
        var contact_with_height =
            in_position.with_height(0);
        var splash = Instantiate(frontal_splash_prefab,contact_with_height, (in_impulse*-1).to_quaternion());
    }


    public void OnCollisionEnter2D(Collision2D other) {
        Projectile collided_projectile = other.gameObject.GetComponent<Projectile>();
        if (collided_projectile != null) {

            Vector2 contact_point = other.GetContact(0).point;

            create_back_splash(
                contact_point,
                other.GetContact(0).relativeVelocity*collided_projectile.GetComponent<Rigidbody2D>().mass
            );
            create_frontal_splash(
                contact_point,
                other.GetContact(0).relativeVelocity*collided_projectile.GetComponent<Rigidbody2D>().mass
            );
            
        }
    }
}
}