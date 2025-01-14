using System.Collections.Generic;
using rvinowise.contracts;
using rvinowise.unity.actions;
using UnityEngine;


namespace rvinowise.unity
{
public class Attacking_group: 
    Abstract_children_group
    ,IAttacker
{

    public List<GameObject> weapon_objects; //only for initialisation in inspector
    public readonly List<Attacker_child_of_group> weapons = new List<Attacker_child_of_group>();
    public override IEnumerable<IChild_of_group> get_children() {
        return weapons;
    }


    internal override void Awake() {
        base.Awake();
        foreach (var weapon_object in weapon_objects) {
            weapons.Add(weapon_object.GetComponent<Attacker_child_of_group>());
        }
    }
    
    
    public override void hide_children_from_copying() {
        base.hide_children_from_copying();
        weapons.Clear();
        weapon_objects.Clear();
    }
    public override void add_child(IChild_of_group in_child) {
        Contract.Requires(in_child is Attacker_child_of_group);
        if (in_child is Attacker_child_of_group attacker)
        {
            weapons.Add(attacker);
            weapon_objects.Add(attacker.gameObject);
            in_child.transform.SetParent(transform, false);
        }
    }

    #region IWeaponry
    public bool is_weapon_ready_for_target(Transform target) {
        foreach (var weapon in weapons) {
            if (weapon.is_weapon_ready_for_target(target)) {
                return true;
            }
        }
        return false;
    }

    private readonly List<Damage_receiver> targets = new List<Damage_receiver>();
    public IEnumerable<Damage_receiver> get_targets() {
        targets.Clear();
        foreach (var weapon in weapons) {
            targets.AddRange(weapon.get_targets());
        }
        return targets;
    }

    public float get_reaching_distance() {
        float max_distance = 0;
        foreach (var weapon in weapons) {
            if (max_distance < weapon.get_reaching_distance()) {
                max_distance = weapon.get_reaching_distance();
            }
        }
        return max_distance;
    }

    public void attack(Transform target, System.Action on_completed) {
        foreach (var weapon in weapons) {
            if (weapon.is_weapon_ready_for_target(target)) {
                weapon.attack(target,on_completed);
            }
        }
    }
    
    #endregion
    
    #region IActor

    public Actor actor { get; set; }

    public void on_lacking_action() {
        
    }

    #endregion
}

}

