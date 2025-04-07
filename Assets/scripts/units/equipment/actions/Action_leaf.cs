#define RVI_DEBUG

using System;
using System.Collections.Generic;
using System.Linq;
using rvinowise.contracts;
using UnityEngine;

namespace rvinowise.unity.actions {

/* Action whith actually moves Actors (other actions are controllers of the execution flow) */
public abstract class Action_leaf:
    Action
{
    internal readonly List<Actor> actors = new List<Actor>();

    
    
    // protected Actor add_actor(Actor actor) {
    //     if (actor == null) {
    //         Debug.LogError($"actor is null");
    //     }
    //     actors.Add(actor);
    //     return actor;
    // }
    protected Actor add_actor(IActing_role role) {
        if (role.actor == null) {
            Debug.LogError($"the actor of role {role} is null");
        }
        
        actors.Add(role.actor);
        return role.actor;
    }

    protected override void on_start_execution() {
        #if RVI_DEBUG
        foreach (var actor in actors) {
            Contract.Requires(
                runner.get_actors().Contains(actor),
                $"the actor {actor}, used in the action {get_explanation()} doesn't exist in its action_runner {runner.name}"
            );
        }
        #endif
        
        base.on_start_execution();
        
    }

    public override void seize_needed_actors_recursive() {
        foreach(Actor seized_actor in actors) {
            if (seized_actor.current_action != null) {
                runner.mark_action_as_finishing(seized_actor.current_action.get_root_action());
            }
            seized_actor.current_action = this;
            
        }
    }

    
    public override void notify_actors_about_finishing_recursive() {
        foreach (Actor actor in actors) {
            actor.on_lacking_action();
        }
    }
    public override void free_actors_recursive() {
        foreach (Actor actor in actors) {
            if (actor == null) {
                Debug.LogError($"actor of action [{this.marker}] is null");
            }
            if (actor.current_action == this) {
                actor.current_action = null;
            }
        }
    }
    
    public override void reset() {
        actors.Clear();
        base.reset();
    }
    
    public override string get_actors_names() {
        var names = new List<string>();
        foreach (var actor in actors) {
            names.Add((actor as MonoBehaviour)?.name);
        }
        return String.Join(", ",names);
    }
    
}



}