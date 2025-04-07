using System.Collections.Generic;
using System.Linq;
using rvinowise.contracts;
using rvinowise.unity.actions;
using rvinowise.unity.extensions;
using UnityEngine;


namespace rvinowise.unity
{
public class Gun_holder: 
    // MonoBehaviour, 
    // IAttacker,IChildren_group
    Attacker_child_of_group,
    ITeam_member
{

    public IGun gun;
    public Turning_element turning_element;
    public Transform muzzle;
    public static LayerMask obstacles_of_shooting;

    public Team team;
    
    private void Awake() {
        gun = GetComponentInChildren<IGun>();
        obstacles_of_shooting= ~LayerMask.GetMask("projectiles");
        team = GetComponentInParent<Intelligence>()?.team;
    }


    public void attach_to_team(Team in_team) {
        this.team = in_team;
    }

    #region IWeaponry
    
    
    public override bool is_weapon_ready_for_target(Transform target) {
        return
            is_weapon_ready_to_shoot()
            &&
            is_weapon_directed_at_target(target);
    }

    public override IEnumerable<Damage_receiver> get_targets() {
        if (is_weapon_ready_to_shoot()) {
            var aiming_target = get_target_of_aiming(muzzle,reaching_distance);
            if ((aiming_target != null)&&
                (aiming_target.GetComponent<Damage_receiver>() is {} damageable) 
                ) {
                
                if (damageable.intelligence == null) {
                    Enumerable.Empty<Damage_receiver>();
                } else
                if (damageable.intelligence.team == null) {
                    Debug.LogError("damageable.intelligence.team == null");
                }
                if (damageable.intelligence?.team?.is_enemy_team(team) == true) {
                    return new[] {damageable};
                }
            }
        }
        return Enumerable.Empty<Damage_receiver>();
    }

    public bool is_weapon_ready_to_shoot() {
        if (gun == null) {
            Debug.LogError($"gun of {name} is null");
        }
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
        var hit = Finding_objects.raycast(
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
    public override float get_reaching_distance() {
        return reaching_distance;
    }

    public override void attack(Transform target, System.Action on_completed) {
        gun.pull_trigger();
        gun.release_trigger();
        on_completed?.Invoke();
    }
    
    #endregion

    #region Children_group

    public IEnumerable<IChild_of_group> get_children() {
        if (gun != null) {
            return new[] {gun};
        }
        return Enumerable.Empty<IChild_of_group>();
    }

    public IList<IChild_of_group> children_stashed_from_copying { get; } = new List<IChild_of_group>();
    public void hide_children_from_copying() {
        gun.transform.SetParent(null,false);
        children_stashed_from_copying.Clear();
        children_stashed_from_copying.Add(gun);
    }

    public void add_child(IChild_of_group child) {
        gun = child as IGun;
        gun?.transform.SetParent(transform, false);
    }

    public void shift_center(Vector2 in_shift) {
        if (gun!=null) {
            gun.transform.localPosition += (Vector3) in_shift;
        }
    }

    public void distribute_data_across(IEnumerable<IChildren_group> new_controllers) {
        
    }

    #endregion //Children_group
    
    #region IActor

    public Actor actor { get; set; }

    public void on_lacking_action() {
        Idle.create(this).start_as_root(actor.action_runner);
    }
    
    #endregion
}

}

