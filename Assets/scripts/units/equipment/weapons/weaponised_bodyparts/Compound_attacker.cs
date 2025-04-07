using System.Collections.Generic;
using System.Linq;
using rvinowise.contracts;
using rvinowise.unity.actions;
using UnityEngine;


namespace rvinowise.unity
{
public class Compound_attacker: 
    IAttacker
{

    public List<GameObject> weapon_objects; //only for initialisation in inspector
    public readonly List<IAttacker> child_attackers;


    // internal override void Awake() {
    //     base.Awake();
    //     foreach (var weapon_object in weapon_objects) {
    //         weapons.Add(weapon_object.GetComponent<Attacker_child_of_group>());
    //     }
    // }

    public Compound_attacker(IEnumerable<IAttacker> weapons) {
        child_attackers = weapons.ToList();
    }
    
    
    #region IWeaponry
    public bool is_weapon_ready_for_target(Transform target) {
        foreach (var weapon in child_attackers) {
            if (weapon.is_weapon_ready_for_target(target)) {
                return true;
            }
        }
        return false;
    }

    private readonly List<Damage_receiver> targets = new List<Damage_receiver>();
    public IEnumerable<Damage_receiver> get_targets() {
        targets.Clear();
        foreach (var weapon in child_attackers) {
            targets.AddRange(weapon.get_targets());
        }
        return targets;
    }

    public float get_reaching_distance() {
        float max_distance = 0;
        foreach (var weapon in child_attackers) {
            if (max_distance < weapon.get_reaching_distance()) {
                max_distance = weapon.get_reaching_distance();
            }
        }
        return max_distance;
    }

    public void attack(Transform target, System.Action on_completed) {
        foreach (var weapon in child_attackers) {
            if (weapon.is_weapon_ready_for_target(target)) {
                weapon.attack(target,on_completed);
                break;
            }
        }
    }
    
    #endregion


    public Actor actor { get; set; }

    public void on_lacking_action() {
        Idle.create(this).start_as_root(actor.action_runner);
    }
}

}

