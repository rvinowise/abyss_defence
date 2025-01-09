using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using rvinowise.contracts;
using rvinowise.unity.actions;
using rvinowise.unity.extensions;
using rvinowise.unity.geometry2d;
using UnityEngine.Serialization;
using Action = rvinowise.unity.actions.Action;


namespace rvinowise.unity {

public class Hitting_limbs_group:
    Abstract_children_group
    ,IAttacker
{
    public List<Hitting_limb> hitting_limbs;


    private System.Action<actions.Action> intelligence_on_complete;
    
    
    
    #region IWeaponry

    public bool is_weapon_ready_for_target(Transform target) {
        foreach (var limb in hitting_limbs) {
            if (limb.is_weapon_ready_for_target(target)) {
                return true;
            }
        }
        return false;
    }

    public float get_reaching_distance() {
        float max_distance = 0;
        foreach (var limb in hitting_limbs) {
            if (max_distance < limb.get_reaching_distance()) {
                max_distance = limb.get_reaching_distance();
            }
        }
        return max_distance;
    }
    
    public void attack(Transform target, System.Action on_completed = null) {
        hitting_limbs.First().attack(target, on_completed);
    }

    
    #endregion
    

    #region IActor

    public Actor actor { get; set; }

    public void on_lacking_action() {
        
    }

    
    #endregion
    
    
    #region Children_group

    public override IEnumerable<IChild_of_group> get_children() {
        return hitting_limbs;
    }

    public override void add_child(IChild_of_group in_child) {
        hitting_limbs.Add(in_child as Hitting_limb);
    }
    
    public override void hide_children_from_copying() {
        base.hide_children_from_copying();
        hitting_limbs.Clear();
    }

    #endregion
}

}