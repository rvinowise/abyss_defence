using System.Collections.Generic;
using System.Linq;
using rvinowise.contracts;
using rvinowise.unity.actions;
using rvinowise.unity.extensions;
using UnityEngine;


namespace rvinowise.unity
{
public class Gun_holder: MonoBehaviour, 
    IAttacker
{

    public IGun gun;
    public Turning_element turning_element;
    public Transform muzzle;
    public static LayerMask obstacles_of_shooting;
    
    private void Awake() {
        gun = GetComponentInChildren<IGun>();
        obstacles_of_shooting= ~LayerMask.GetMask("projectiles");
    }
    
    
    #region IWeaponry
    
    
    public bool is_weapon_ready_for_target(Transform target) {
        return
            is_weapon_ready_to_shoot()
            &&
            is_weapon_directed_at_target(target);
    }

    public bool is_weapon_ready_to_shoot() {
        return gun.can_fire();
    }

    public bool is_weapon_directed_at_target(Transform target) {
        var hit = Physics2D.Raycast(
            muzzle.position, 
            muzzle.rotation.to_vector(),
            reaching_distance,
            obstacles_of_shooting
        );

        if (hit.transform == target) {
            return true;
        }
        return false;
    }

    public float reaching_distance;
    public float get_reaching_distance() {
        return reaching_distance;
    }

    public void attack(Transform target, System.Action on_completed) {
        gun.pull_trigger();
        gun.release_trigger();
        on_completed.Invoke();
    }
    
    #endregion

    #region IActor

    public Actor actor { get; set; }

    public void on_lacking_action() {
        Idle.create(actor).start_as_root(actor.action_runner);
    }
    
    #endregion
}

}

