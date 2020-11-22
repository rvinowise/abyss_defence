using System;
using UnityEngine;
using rvinowise.unity.extensions;
using rvinowise.unity.geometry2d;
using rvinowise.unity.effects.liquids;
using rvinowise.unity.effects.physics;
using rvinowise.rvi.contracts;
using rvinowise.unity.units.parts.weapons.guns.common;

using Random = UnityEngine.Random;


namespace rvinowise.unity.units.gore {

public class Bleeding_body: MonoBehaviour {

    /*[SerializeField] 
    public Object_pool<Droplet> object_pool;*/

    [SerializeField] public Droplet droplet_prefab;

    public void Awake() {
        bool test = true;
    }


    private float spread_degrees = 15f;
    private Vector2 rotation_to_speed_ratio = new Vector2(30f, 40f);
    public void create_splash(
        Vector2 in_position,
        Vector2 in_impulse,
        int in_size
    ) {
        
        
        for (int i=0;i<in_size;i++) {
            Degree rotation_aside = new Degree(Random.Range(-spread_degrees, spread_degrees));
            Vector2 rotated_impulse = in_impulse.rotate(rotation_aside.degrees);

            float speed_preservation = Math.Max(0,
                1 - Mathf.Abs(rotation_aside.degrees) /
                Random.Range(rotation_to_speed_ratio.x, rotation_to_speed_ratio.y)
            );
            
            Vector2 final_impulse = rotated_impulse * speed_preservation;
            
            bleed_a_droplet(
                in_position,
                final_impulse
            );
        }
        
        
    }
    
    private Droplet bleed_a_droplet(
        Vector2 in_position,
        Vector2 in_impulse
    ) {
        Droplet droplet = droplet_prefab.get_from_pool<Droplet>();
        droplet.transform.move_preserving_z(in_position);
        Trajectory_flyer trajectory_flyer = droplet.GetComponent<Trajectory_flyer>();
        trajectory_flyer.height = 0.2f;
        trajectory_flyer.vertical_velocity = 1f + Random.value*3;
        droplet.size = Random.Range(0.5f, 1.5f);
        trajectory_flyer.weight = droplet.size * 10;

        Degree rotation_aside = new Degree(Random.Range(-spread_degrees, spread_degrees));
        Vector2 rotated_impulse = in_impulse.rotate(rotation_aside.degrees);
        Vector2 impulse = rotated_impulse / (
                              1 + Mathf.Abs(rotation_aside.degrees) /
                              Random.Range(rotation_to_speed_ratio.x, rotation_to_speed_ratio.y)
                          );
            
        droplet.rigidbody.AddForce(impulse*Time.deltaTime/10f,ForceMode2D.Impulse);
        droplet.transform.rotation = impulse.to_quaternion();
            
        

        return droplet;
    }

    
    public void OnCollisionEnter2D(Collision2D other) {
        Debug.Log("OnCollisionEnter2D in "+this.gameObject.name);
        Projectile collided_projectile = other.gameObject.GetComponent<Projectile>();
        if (collided_projectile != null) {

            Vector2 contact_point = other.GetContact(0).point;
            Ray2D ray_of_impact = new Ray2D(
                contact_point, other.GetContact(0).relativeVelocity
            );

            create_splash(
                contact_point,
                other.GetContact(0).relativeVelocity/10000,
                10
            );
            
        }
    }
}
}