using System.Collections.Generic;
using System.Linq;
using rvinowise.contracts;
using rvinowise.unity.actions;
using UnityEngine;


namespace rvinowise.unity
{
public class Compound_attacker: 
    IActor_attacker
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
    public bool is_weapon_targeting_target(Transform target) {
        foreach (var weapon in child_attackers) {
            if (weapon.is_weapon_targeting_target(target)) {
                return true;
            }
        }
        return false;
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
            if (weapon.is_weapon_targeting_target(target)) {
                weapon.attack(target,on_completed);
                break;
            }
        }
    }
    
    #endregion

    public void init_for_runner(Action_runner action_runner) {
        this.action_runner = action_runner;
    }

    private Action_runner action_runner;

    public Action current_action { get; set; }
    public void on_lacking_action() {
        Idle.create(this).start_as_root(action_runner);
    }
}

}

