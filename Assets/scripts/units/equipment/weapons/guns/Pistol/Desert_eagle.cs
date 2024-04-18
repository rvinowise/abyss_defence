using rvinowise.unity.extensions;
using rvinowise.unity.geometry2d;
using UnityEngine;
using Random = UnityEngine.Random;
using rvinowise.unity.extensions.attributes;

namespace rvinowise.unity {

public class Desert_eagle: Pistol {

    public override float stock_length { get; } = 0f;


    private static readonly int animation_fire = Animator.StringToHash("fire");

    private float frames_to_shell_ejection = 15f;


    
    protected override void fire() {
        var new_projectile = base.get_projectile();
        if (new_projectile == null) {
            return;
        }
        
        animator.SetTrigger("slide");
        propell_projectile(new_projectile);
    }

    public float projectile_force = 1000f;//100f;

    private void propell_projectile(Projectile projectile) {
        Rigidbody2D rigid_body = projectile.GetComponent<Rigidbody2D>();
        rigid_body.AddForce(transform.rotation.to_vector() * (projectile_force * Time.deltaTime), ForceMode2D.Impulse);
        projectile.store_last_physics();
    }


    /* invoked from an animation */
    [called_in_animation]
    private void eject_bullet_shell() {
        
        Gun_shell new_shell = bullet_shell_prefab.get_from_pool<Gun_shell>(
            shell_ejector.position,
            transform.rotation
        );
        new_shell.enabled = true;
            
        const float ejection_force = 5f;
        Vector2 ejection_vector = Directions.degrees_to_quaternion(-15+Random.value*30) *
                                  shell_ejector.right *
                                  ejection_force;
        
        Vector2 gun_vector = (Vector2)this.transform.position - last_physics.position;
        
        var shell_rigidbody = new_shell.GetComponent<Rigidbody2D>();
        shell_rigidbody.AddForce(ejection_vector + gun_vector*50);
        shell_rigidbody.AddTorque(-360f + Random.value*300f);
        
        Trajectory_flyer flyer = new_shell.GetComponent<Trajectory_flyer>();
        flyer.height = 1;
        flyer.vertical_velocity = 1f + Random.value * 3f;
    }
}
}