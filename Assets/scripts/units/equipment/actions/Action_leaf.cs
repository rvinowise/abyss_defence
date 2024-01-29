using System.Collections.Generic;
using UnityEngine;


namespace rvinowise.unity.units.parts.actions {

/* Action whith actually moves Actors (other actions are controllers of the execution flow) */
public abstract class Action_leaf:
    Action
{
    protected List<IActor> actors = new List<IActor>();

    public IActor add_actor(IActor actor) {
        actors.Add(actor);
        return actor;
    }
    
    
    public override void seize_needed_actors_recursive() {
        foreach(IActor seized_actor in actors) {
            if (seized_actor.current_action != null) {
                runner.mark_action_as_finishing(seized_actor.current_action.get_root_action());
            }
            seized_actor.current_action = this;
        }
    }
    
    public override void notify_actors_about_finishing() {
        foreach (IActor actor in actors) {
            actor.on_lacking_action();
        }
    }
    public override void free_actors_recursive() {
        foreach (IActor actor in actors) {
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
    
}



}