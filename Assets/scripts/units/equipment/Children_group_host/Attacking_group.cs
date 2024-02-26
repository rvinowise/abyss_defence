using System.Collections.Generic;
using rvinowise.contracts;
using UnityEngine;


namespace rvinowise.unity
{
public class Attacking_group: 
    Children_group
    ,IAttacker
{

    public List<GameObject> weapon_objects; //only for initialisation in inspector
    public readonly List<IAttacker_child_of_group> weapons = new List<IAttacker_child_of_group>();
    public override IEnumerable<IChild_of_group> children {
        get => weapons;
    }


    protected override void Awake() {
        base.Awake();
        foreach (var weapon_object in weapon_objects) {
            weapons.Add(weapon_object.GetComponent<IAttacker_child_of_group>());
        }
    }
    
    
    public override void hide_children_from_copying() {
        base.hide_children_from_copying();
        weapons.Clear();
    }
    public override void add_child(IChild_of_group in_child) {
        Contract.Requires(in_child is IAttacker_child_of_group);
        if (in_child is IAttacker_child_of_group attacker)
        {
            weapons.Add(attacker);
            in_child.transform.SetParent(transform, false);
        }
    }

    #region IWeaponry
    public bool can_reach(Transform target) {
        foreach (var weapon in weapons) {
            if (weapon.can_reach(target)) {
                return true;
            }
        }
        return false;
    }

    public void attack(Transform target, System.Action on_completed) {
        foreach (var weapon in weapons) {
            if (weapon.can_reach(target)) {
                weapon.attack(target,on_completed);
            }
        }
    }
    
    #endregion
}

}

