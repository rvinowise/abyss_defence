using System;
using System.Collections.Generic;
using System.Linq;
using rvinowise.unity.extensions;
using rvinowise.unity.geometry2d;
using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;


namespace rvinowise.unity {

[RequireComponent(typeof(SpriteRenderer))]

public class Bleeding_body_droplet_objects:
    MonoBehaviour,
    IBleeding_body
{

    public Trajectory_flyer droplet_prefab;
    
    
    private float spread_degrees = 15f;
    private Vector2 rotation_to_speed_ratio = new Vector2(30f, 40f);
    
    
    private void Awake() {
        int test = 1;
    }
    
   
    
    public void create_splash(
        Vector2 in_position,
        Vector2 in_impulse
    ) {
        create_splash(in_position, in_impulse, 5);
    }
    
    
    
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
            
            emit_a_particle(
                in_position,
                final_impulse
            );
        }
    }
    
    private Trajectory_flyer emit_a_particle(
        Vector2 in_position,
        Vector2 in_impulse
    ) {
        Trajectory_flyer trajectory_flyer = droplet_prefab.get_from_pool<Trajectory_flyer>();
        trajectory_flyer.transform.position = in_position;
        var droplet_height = -transform.position.z + 0.1f;
        
        trajectory_flyer.height = droplet_height;
        trajectory_flyer.vertical_velocity = 0.1f + Random.value*2;
        trajectory_flyer.size = Random.Range(0.1f, 0.5f);
        trajectory_flyer.weight = trajectory_flyer.size * 200;
    
        Degree rotation_aside = new Degree(Random.Range(-spread_degrees, spread_degrees));
        Vector2 rotated_impulse = in_impulse.rotate(rotation_aside.degrees);
        Vector2 impulse = rotated_impulse / (
                              1 + Mathf.Abs(rotation_aside.degrees) /
                              Random.Range(rotation_to_speed_ratio.x, rotation_to_speed_ratio.y)
                          );
        
        var particle_rigidbody = trajectory_flyer.GetComponent<Rigidbody2D>();
        particle_rigidbody.AddForce(impulse*Time.deltaTime/10f,ForceMode2D.Impulse);
        trajectory_flyer.transform.rotation = impulse.to_quaternion();
    
        assign_color_to_particle(trajectory_flyer);
        assign_fading_speed_to_particle(trajectory_flyer);
        
        return trajectory_flyer;
    }
    
    private void assign_color_to_particle(Trajectory_flyer particle) {
        var sprite_renderer = particle.GetComponent<SpriteRenderer>();
        sprite_renderer.color = new Color(0.5f+Random.value, Random.value*0.5f,Random.value*0.5f,1);
    }
    
    private void assign_fading_speed_to_particle(Trajectory_flyer particle) {
        var fading_piece = particle.GetComponent<Fading_piece>();
        fading_piece.final_alpha = 0.8f + Random.value*0.2f;
        fading_piece.alpha_change = 0.1f + Random.value;
    }
    
    
    public void OnCollisionEnter2D(Collision2D other) {
        Projectile collided_projectile = other.gameObject.GetComponent<Projectile>();
        if (collided_projectile != null) {
    
            Vector2 contact_point = other.GetContact(0).point;
            Ray2D ray_of_impact = new Ray2D(
                contact_point, other.GetContact(0).relativeVelocity
            );
    
            create_splash(
                contact_point,
                other.GetContact(0).relativeVelocity*collided_projectile.GetComponent<Rigidbody2D>().mass,
                (int) (10*collided_projectile.damage_dealer.effect_amount)
            );
            
        }
    }


    public void receive_damage(Vector2 contact_point, Vector2 impact_impulse, Vector2 impact_normal, float strenght) {
        throw new NotImplementedException();
    }

}
}