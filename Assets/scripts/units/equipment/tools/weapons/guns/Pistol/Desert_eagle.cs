using System;
using System.Collections;
using System.Collections.Generic;

using rvinowise.unity.extensions.pooling;
using rvinowise.unity.geometry2d;
using UnityEngine;
using rvinowise.unity.extensions;

using rvinowise;
using rvinowise.unity.effects.physics;
using rvinowise.unity.units.parts.weapons.guns.common;
using Random = UnityEngine.Random;


namespace rvinowise.unity.units.parts.weapons.guns {

public class Desert_eagle: Pistol {

    //public override float weight { set; get; } = 1f;
    public override float stock_length { get; } = 0f;

    private Saved_physics last_physics = new Saved_physics();


    void Awake() {
        base.Awake();
        
        insert_magazine(
            GameObject.Instantiate(magazine_prefab).GetComponent<Magazine>()    
        );
    }
    
    
    void LateUpdate()
    {
        last_physics.position = transform.position;
    }



    private static readonly int animation_fire = Animator.StringToHash("fire");

    private float frames_to_shell_ejection = 15f;


    public static float recoil_force = 0.15f;
    protected override Projectile fire() {
        var new_projectile = base.fire();
        if (new_projectile == null) {
            return null;
        }
        
        animator.SetTrigger("slide");
        propell_projectile(new_projectile);
        
        this.recoil_receiver.velocity += -transform.rotation.to_vector() * recoil_force;
        
        return new_projectile;
    }

    public float projectile_force = 100f;//100f;

    private void propell_projectile(Projectile projectile) {
        Rigidbody2D rigid_body = projectile.GetComponent<Rigidbody2D>();
        rigid_body.AddForce(transform.rotation.to_vector() * projectile_force * Time.deltaTime, ForceMode2D.Impulse);
        projectile.store_last_physics();
    }


    /* invoked from an animation */
    private void eject_bullet_shell() {
        float ejection_force = 5f;
        Bullet_shell new_shell = bullet_shell_prefab.get_from_pool<Bullet_shell>(
            shell_ejector.position,
            transform.rotation
        );
        new_shell.enabled = true;
            
        Vector2 ejection_vector = Directions.degrees_to_quaternion(-15+Random.value*30) *
                                  shell_ejector.right *
                                  ejection_force;
        
        Vector2 gun_vector = (Vector2)this.transform.position - last_physics.position;
        
        var rigidbody = new_shell.GetComponent<Rigidbody2D>();
        rigidbody.AddForce(ejection_vector + gun_vector*50);
        rigidbody.AddTorque(-360f + Random.value*300f);
        
        Trajectory_flyer flyer = new_shell.GetComponent<Trajectory_flyer>();
        flyer.height = 1;
        flyer.vertical_velocity = 1f + Random.value * 3f;
    }
}
}