using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using rvinowise.contracts;
using rvinowise.unity.actions;
using rvinowise.unity.extensions;
using UnityEngine.Serialization;
using Action = rvinowise.unity.actions.Action;


namespace rvinowise.unity {

public class Scorpion_pedipalps_group :
    Abstract_children_group,
    IActor_attacker,
    IActor_defender
{
    public List<Scorpion_pedipalp> pedipalps = new List<Scorpion_pedipalp>();

    public override IEnumerable<IChild_of_group> get_children() {
        return pedipalps;
    }

    public override void add_child(IChild_of_group in_child) {
        var added_child = in_child as Scorpion_pedipalp;
        pedipalps.Add(added_child);
        added_child.transform.SetParent(transform, false);
    }
    
    public override void hide_children_from_copying() {
        base.hide_children_from_copying();
        pedipalps.Clear();
    }

    public bool can_reach(Transform target) {
        foreach (var pedipalp in pedipalps) {
            return pedipalp.can_reach(target);
        }
        return false;
    }

    public float get_reaching_distance() {
        float max_distance = 0;
        foreach (var arm in pedipalps) {
            if (max_distance < arm.get_reaching_distance()) {
                max_distance = arm.get_reaching_distance();
            }
        }
        return max_distance; 
    }

    public bool is_target_at_fighting_distance(Scorpion_pedipalp arm, Transform target) {
        var distance_to_target = (target.position - arm.transform.position).magnitude;
        return arm.get_length()*2 >= distance_to_target;
    }

    public void attack(Transform target, System.Action on_completed = null) {
        foreach (var pedipalp in pedipalps) {
            if (pedipalp.can_reach(target)) {
                Scorpion_arm_attack.create(
                    pedipalp,
                    target
                ).set_on_completed(on_completed)
                .start_as_root(action_runner);
            }
            return;
        }
        
    }

    public void start_defence(Transform target, System.Action on_completed) { }

    public void finish_defence(System.Action on_completed) { }

    #region IActor

    private Action_runner action_runner;

    public void init_for_runner(Action_runner in_action_runner) {
        this.action_runner = in_action_runner;
    }

    public Action current_action { get; set; }
    public void on_lacking_action() {
        Idle.create(this).start_as_root(action_runner);
    }

    #endregion IActor

}

}