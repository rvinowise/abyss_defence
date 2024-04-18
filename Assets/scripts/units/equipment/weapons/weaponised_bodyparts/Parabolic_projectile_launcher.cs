using System;
using System.Collections.Generic;
using rvinowise.unity.extensions;
using rvinowise.unity.extensions.attributes;
using UnityEngine;
using Action = rvinowise.unity.actions.Action;


namespace rvinowise.unity {
public class Parabolic_projectile_launcher : 
    Attacker_child_of_group 
{

    public Droplet projectile_prefab;
    public Transform muzzle_position;
    public float launching_impulse = 2;
    public float launching_distance = 2;

    public float cooldown_seconds = 1;
    private float last_shot_time = float.MinValue;
    
    #region IWeaponry interface
    public override bool can_reach(Transform target) {
        var distance_to_target =
            transform.position.distance_to(target.position); 
        var angle_to_target =
            transform.rotation.to_degree().angle_to(transform.position.degrees_to(target.position));

        return 
            distance_to_target <= launching_distance
            &&
            Math.Abs(angle_to_target.degrees) <= 10f;
    }
    
    public override float get_reaching_distance() {
        return launching_distance;
    }
    

    public override void attack(Transform target, System.Action on_completed = null) {
        if (is_ready_to_attack()) {
            launch_projectile(transform.position.distance_to(target.position));
            last_shot_time = Time.time;
        }
        on_completed?.Invoke();
    }
    
    #endregion IWeaponry interface

    private bool is_ready_to_attack() {
        return Time.time - last_shot_time > cooldown_seconds;
    }
    
    private Droplet launch_projectile(
        float landing_distance
    ) {
        Droplet droplet = Instantiate(projectile_prefab);
        droplet.transform.move_preserving_z(muzzle_position.position);
        
        Trajectory_flyer trajectory_flyer = droplet.GetComponent<Trajectory_flyer>();
        trajectory_flyer.height = 0.5f;
        trajectory_flyer.vertical_velocity = 
            trajectory_flyer.get_vertical_impulse_for_landing_at_distance(
                landing_distance,
                launching_impulse
            );

        var impulse = transform.rotation.to_vector() * launching_impulse;
        droplet.rigidbody.AddForce(impulse,ForceMode2D.Impulse);
        droplet.transform.rotation = impulse.to_quaternion();
            
        return droplet;
    }
   
    
}

}