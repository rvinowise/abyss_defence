using rvinowise.unity.geometry2d;
using UnityEngine;
using rvinowise.unity.extensions;
using rvinowise.unity.units.parts.weapons.guns.common;
using System.Diagnostics.Contracts;
using Random = UnityEngine.Random;
using rvinowise.unity.effects.physics;

namespace rvinowise.unity.units.parts.weapons.guns {

public class Break_shotgun: Gun {

    public Transform shell_ejector;

    /* protected override void init_holding_places() {
        main_holding = Holding_place.main(this);
        second_holding = Holding_place.create(this.transform);
        second_holding.place_on_tool = new Vector2(0.2f, 0f);
        second_holding.grip_direction = new Degree(-80f);
        second_holding.grip_gesture = Hand_gesture.Support_of_horizontal;
    } */
    
    public int projectiles_qty = 8;

    protected override void fire() {
        Contract.Requires(can_fire(), "function Fire must be invoked after making sure it's possible");
        last_shot_time = Time.time;

        for (int i=0;i<projectiles_qty;i++) {
            Projectile new_projectile = projectile_prefab.get_from_pool<Projectile>(
                muzzle.position, muzzle.rotation
            );
            propell_projectile(new_projectile);
        }
        ammo_qty -=1;
        Transform new_spark = spark_prefab.get_from_pool<Transform>();
        new_spark.copy_physics_from(muzzle);
        
        this.recoil_receiver?.push_with_recoil(recoil_force);
        notify_that_ammo_changed();
    }

    public float projectile_force = 100f;
    private void propell_projectile(Projectile projectile) {
        Rigidbody2D rigid_body = projectile.GetComponent<Rigidbody2D>();
        Vector2 randomization = Vector2.one;//new Vector3(-2 + Random.value * 4f, -2 + Random.value *4f, 0f);
        rigid_body.AddForce(
            transform.rotation.to_vector() * 
            randomization *
            projectile_force * 
            Time.deltaTime, 
            ForceMode2D.Impulse);
        projectile.store_last_physics();
    }

    public void eject_shells() {
        
        Gun_shell new_shell = bullet_shell_prefab.get_from_pool<Gun_shell>(
            shell_ejector.position,
            transform.rotation
        );
        new_shell.enabled = true;
            
        float ejection_force = 5f;
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