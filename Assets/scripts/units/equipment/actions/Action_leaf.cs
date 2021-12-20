using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using rvinowise.debug;
using rvinowise.contracts;
using rvinowise.unity.units.control;
using rvinowise.unity.units.parts.limbs.arms.actions;
using UnityEngine.Animations;
using Debug = UnityEngine.Debug;
using rvinowise.unity.units.humanoid;

namespace rvinowise.unity.units.parts.actions {

/* Action whith actually moves Actors (other actions are controllers of the execution flow) */
public abstract class Action_leaf:
    Action
{


    protected List<IActor> actors = new List<IActor>();
    protected List<Action> superceded_actions = new List<Action>();

    public IActor add_actor(IActor actor) {
        actors.Add(actor);
        return actor;
    }

    public IActor actor {
        get {
            Contract.Assume(actors.Count() == 1);
            return actors.First();
        }
        set {
            add_actor(value);
        }
    }
    
    public override void seize_needed_actors_recursive() {
        Contract.Assert(actors.Any());
        foreach(IActor actor in actors) {
            superceded_actions.Add(actor.current_action);
            actor.current_action = this;
        }
    }

    public override void ensure_actors_have_next_action() {
        foreach (IActor actor in actors) {
            Contract.Assert(actor.current_action != this, 
                "this function is called after the action has freed its actors");
            if (actor.current_action == null) {
                actor.on_lacking_action();
            }
        }
    }
    
    public override void free_actors_recursive() {
        // if !actor_is_seased_by_another_action(actor)
        foreach (IActor actor in actors) {
            if (actor.current_action == this) {
                actor.current_action = null;
            }
        }
    }

    public override void notify_actors_about_finishing() {
        foreach (IActor actor in actors) {
            actor.on_lacking_action();
        }
    }
    
    public override void reset() {
        actors.Clear();
        superceded_actions.Clear();
        base.reset();
    }
}



}