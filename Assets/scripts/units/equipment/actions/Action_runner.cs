


using System.Collections.Generic;
using System.Linq;
using rvinowise.contracts;
using UnityEngine;
using UnityEngine.Assertions;

namespace rvinowise.unity.units.parts.actions {

public class Action_runner {
    
    public List<IActor> actors = new List<IActor>();
    
    public List<Action> current_actions = new List<Action>();
    public ISet<Action> finishing_actions = new HashSet<Action>();
    public List<Action> starting_actions = new List<Action>();
    //public List<Action> discarded_actions = new List<Action>();
    public void add_action(Action action) {
        current_actions.Add(action);
    }

    public void mark_action_as_finishing(Action action) {
              
        finishing_actions.Add(action);
    }
    public void mark_action_as_starting(Action action) {
        Contract.Assert(!starting_actions.Contains(action), 
            "action is marked twice!"
        );        
        starting_actions.Add(action);
    }
    public void add_actor(IActor actor) {
        actors.Add(actor);
    }

    
    public void update() {
        if (finishing_actions.Any()) {
            finish_actions(); // sequences & roots
            if (starting_actions.Any()) {
                start_actions(); // sequences
                if (finishing_actions.Any()) {
                    finish_actions(); // superceded by started
                }
            }
        }
        start_fallback_actions();
        foreach (Action action in current_actions) {
            action.update();
        }
    }

    public void start_root_action(Action action) {
        current_actions.Add(action);
        
        action.init_children_recursive();
        action.set_root_action(action);
        action.init_state_recursive();
        action.seize_needed_actors_recursive();
        if (finishing_actions.Any()) {
            finish_actions(); // superceded by started
        }
        start_fallback_actions();
    }

    public void finish_action(Action action) {
        action.restore_state_recursive();
        action.free_actors_recursive();
        action.reset_recursive();
    }
    private void start_action(Action action) {
        action.init_children_recursive();
        action.init_state_recursive();
        action.seize_needed_actors_recursive();
    }

    private void supercede_actions_which_use_same_actors() {
        foreach (Action action in current_actions) {
            if (action.superceded_as_root) {
                finish_action(action);
            }
        }
        current_actions.RemoveAll(action => action.superceded_as_root);
    }
    
    private void finish_actions() {
        foreach (Action completed_action in finishing_actions) {
            finish_action(completed_action);
        }
        current_actions.RemoveAll(action => action.is_reset);
        finishing_actions.Clear();
    }
    private void start_actions() {
        foreach (Action action in starting_actions) {
            start_action(action);
        }
        starting_actions.Clear();
    }


    public void start_fallback_actions() {
        foreach (IActor actor in actors) {
            if (actor.current_action == null) {
                actor.on_lacking_action();
            }
        } 
    }
    
    
    
    public void on_root_action_completed(Action in_action) {
        finishing_actions.Add(in_action);
    }



}
}