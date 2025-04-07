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
        if (back_splash_prefab == null) {
            return;
        }
        var contact_with_height =
            in_position.with_height(this.transform.position.z-0.5f);
            //in_position.with_height(0);
        var splash = Instantiate(
            back_splash_prefab,
            contact_with_height,
            (in_impulse).to_quaternion()
        );
    }
    
    public void create_frontal_splash(
        Vector2 in_position,
        Vector2 in_impulse
    ) {
        if (frontal_splash_prefab == null) {
            return;
        }
        var contact_with_height =
            in_position.with_height(0);
        var splash = Instantiate(
            frontal_splash_prefab,
            contact_with_height,
            (in_impulse).to_quaternion()
        );
    }


    public void OnCollisionEnter2D(Collision2D other) {
        var damaging_collider = other.gameObject.GetComponent<Damaging_collider>();
        if (damaging_collider != null) {

            Vector2 contact_point = other.GetContact(0).point;

            receive_damage(
                contact_point,
                other.GetContact(0).relativeVelocity,
                other.GetContact(0).normal,
                damaging_collider.GetComponent<Rigidbody2D>().mass
            );

        }
    }

    public void receive_damage(
        Vector2 contact_point, 
        Vector2 impact_impulse, 
        Vector2 impact_normal, 
        float strenght
    ) {
        create_back_splash(
            contact_point,
            impact_impulse*strenght
        );
        // create_frontal_splash(
        //     contact_point,
        //     other.GetContact(0).relativeVelocity*collided_projectile.GetComponent<Rigidbody2D>().mass
        // );
        create_frontal_splash(
            contact_point,
            -impact_normal
        );
    }
}
}