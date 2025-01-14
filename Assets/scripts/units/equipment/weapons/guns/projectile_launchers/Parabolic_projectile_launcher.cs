using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using rvinowise.unity.actions;
using rvinowise.unity.extensions;
using rvinowise.unity.extensions.attributes;
using UnityEngine;
using UnityEngine.Serialization;
using Action = rvinowise.unity.actions.Action;


namespace rvinowise.unity {
public class Parabolic_projectile_launcher : 
    Attacker_child_of_group
{

    public Trajectory_flyer projectile_prefab;
    public Transform muzzle;
    public float launching_impulse = 2;
    public float launching_distance = 2;

    public float cooldown_seconds = 1;
    private float last_shot_time = float.MinValue;
    
    #region IWeaponry interface
    public override bool is_weapon_ready_for_target(Transform target) {
        var distance_to_target =
            transform.position.distance_to(target.position); 
        var angle_to_target =
            transform.rotation.to_degree().angle_to(transform.position.degrees_to(target.position));

        return 
            distance_to_target <= launching_distance
            &&
            Math.Abs(angle_to_target.degrees) <= 10f;
    }

    public override IEnumerable<Damage_receiver> get_targets() {
        return Enumerable.Empty<Damage_receiver>();
    }

    public override float get_reaching_distance() {
        return launching_distance;
    }
    

    public override void attack(Transform target, System.Action on_completed = null) {
        if (is_ready_to_attack()) {
            Trajectory_flyer projectile = Instantiate(projectile_prefab);
            launch_projectile(projectile, transform.position.distance_to(target.position));
            last_shot_time = Time.time;
        }
        on_completed?.Invoke();
    }
    
    #endregion IWeaponry interface

    
    private bool is_ready_to_attack() {
        return Time.time - last_shot_time > cooldown_seconds;
    }

    
    public void launch_projectile(
        Trajectory_flyer projectile,
        float landing_distance
    ) {
        projectile.transform.move_preserving_z(muzzle.position);
        projectile.enabled = true;
        
        projectile.height = -muzzle.position.z;
        projectile.vertical_velocity = 
            projectile.get_vertical_impulse_for_landing_at_distance(
                landing_distance,
                launching_impulse
            );

        var impulse = transform.rotation.to_vector() * launching_impulse;
        projectile.transform.rotation = impulse.to_quaternion();
        
        Rigidbody2D projectile_rigidbody = projectile.GetComponent<Rigidbody2D>();
        projectile_rigidbody.AddForce(impulse,ForceMode2D.Impulse);
    }
   
    
}

}