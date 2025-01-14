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

    public Intelligence intelligence;
    
    private void Awake() {
        gun = GetComponentInChildren<IGun>();
        obstacles_of_shooting= ~LayerMask.GetMask("projectiles");
        intelligence = GetComponentInParent<Intelligence>();
    }
    
    
    #region IWeaponry
    
    
    public bool is_weapon_ready_for_target(Transform target) {
        return
            is_weapon_ready_to_shoot()
            &&
            is_weapon_directed_at_target(target);
    }

    public IEnumerable<Damage_receiver> get_targets() {
        if (is_weapon_ready_to_shoot()) {
            var aiming_target = get_target_of_aiming(muzzle,reaching_distance);
            if (aiming_target.GetComponent<Damage_receiver>() is {} damageable) {
                if (damageable.intelligence.team.is_enemy_team(intelligence.team)) {
                    return new[] {damageable};
                }
            }
        }
        return Enumerable.Empty<Damage_receiver>();
    }

    public bool is_weapon_ready_to_shoot() {
        return gun.can_fire();
    }

    public bool is_weapon_directed_at_target(Transform target) {
        if (get_target_of_aiming(muzzle,reaching_distance) == target) {
            return true;
        }
        return false;
    }
    public static Transform get_target_of_aiming(
        Transform muzzle,
        float reaching_distance
    ) {
        var hit = Physics2D.Raycast(
            muzzle.position, 
            muzzle.rotation.to_vector(),
            reaching_distance,
            obstacles_of_shooting
        );

        if (hit.collider?.transform is {} target) {
            return target;
        }
        return null;
    }

    public float reaching_distance;
    public float get_reaching_distance() {
        return reaching_distance;
    }

    public void attack(Transform target, System.Action on_completed) {
        gun.pull_trigger();
        gun.release_trigger();
        on_completed?.Invoke();
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

