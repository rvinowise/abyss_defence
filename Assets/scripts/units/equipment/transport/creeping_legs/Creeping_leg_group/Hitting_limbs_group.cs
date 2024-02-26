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
    Children_group
    ,IAttacker
    ,IRunning_actions
{
    public List<Hitting_limb> hitting_limbs;
    public Creeping_leg_group creeping_leg_group;

    private Action_runner action_runner;

    private System.Action<actions.Action> intelligence_on_complete;
    
    
    
    #region IWeaponry

    public bool can_reach(Transform target) {
        foreach (var limb in hitting_limbs) {
            if (limb.can_reach(target)) {
                return true;
            }
        }
        return false;
    }

    public void attack(Transform target, System.Action on_completed = null) {
        hitting_limbs.First().attack(target, on_completed);
    }

    
    #endregion
    

    #region IRunning_actions

    public void init_for_runner(Action_runner action_runner) {
        this.action_runner = action_runner;
    }
    
    #endregion
    
    
    #region Children_group

    public override IEnumerable<IChild_of_group> children {
        get => hitting_limbs;
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