using System;
using System.Collections.Generic;
using System.Linq;
using rvinowise.unity.actions;
using UnityEngine;
using Action = rvinowise.unity.actions.Action;

namespace rvinowise.unity {
    
public class Compound_defender:
    IDefender
{
    public IList<IDefender> child_defenders;

    public Compound_defender(IEnumerable<IDefender> in_child_defenders) {
        child_defenders = in_child_defenders.ToList();
    }    

    public void start_defence(Transform target, System.Action on_completed) {
        var defending_actions = new List<Action>();
        
        foreach (var child_defender in child_defenders) {
            defending_actions.Add(
                Defender_start_defending.create(
                    child_defender,
                    target
                )  
            );
        }
        
        Action_parallel_parent.create_from_actions(
            defending_actions
        ).set_on_completed(on_completed)
            .start_as_root(actor.action_runner);
    }

    public void finish_defence(System.Action on_completed) {
        var finishing_actions = new List<Action>();
        
        foreach (var child_defender in child_defenders) {
            finishing_actions.Add(
                Defender_finish_defending.create(
                    child_defender
                )  
            );
        }
        
        Action_parallel_parent.create_from_actions(
            finishing_actions
        ).set_on_completed(on_completed)
            .start_as_root(actor.action_runner);
    }
    
    #region IActor

    public Actor actor { get; set; }

    public void on_lacking_action() {
        Idle.create(this).start_as_root(actor.action_runner);
    }

    #endregion
    
}

}