using System;
using UnityEngine;
using rvinowise.unity.extensions;
using rvinowise.contracts;
using rvinowise.unity.extensions.attributes;
using rvinowise.unity.geometry2d;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;


namespace rvinowise.unity {

public class Direct_projectile_launcher: MonoBehaviour
{
    
    
    public Rigidbody2D projectile_prefab;
    public float projectile_force = 1000f;
    public Transform muzzle;
    public Transform spark_prefab;
    public Tool tool;

    public Rigidbody2D get_projectile() {
        Rigidbody2D new_projectile = projectile_prefab.instantiate<Rigidbody2D>(
            muzzle.position, muzzle.rotation
        );
        
        Transform new_spark = spark_prefab.get_from_pool<Transform>();
        new_spark.copy_physics_from(muzzle);
        
        return new_projectile;
    }
    
    public void fire() {
        var new_projectile = get_projectile();
        if (new_projectile == null) {
            return;
        }
        //new_projectile.damage_dealer?.set_attacker(tool.main_holding.holding_hand.arm.pair.user.transform);
        
        
        propell_projectile(new_projectile);
    }
    
    public void propell_projectile(Rigidbody2D projectile) {
        projectile.AddForce(transform.rotation.to_vector() * (projectile_force * Time.deltaTime), ForceMode2D.Impulse);
        //projectile.store_last_physics();
    }
    
}
}