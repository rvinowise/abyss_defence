using System;
using System.Collections.Generic;
using System.Linq;
using rvinowise.debug;
using rvinowise.contracts;


namespace rvinowise.unity.units.parts.actions {

public abstract partial class Action
{
    public Action parent_action;

    protected Action root_action;

    public bool completed { protected set; get; }
    private bool is_free_in_pool;
    private System.Action<Action> on_finished;
    public Action_runner runner;

    protected List<Action> superceded_actions = new List<Action>();
    protected List<IActor> actors = new List<IActor>();

    public IActor add_actor(IActor actor) {
        actors.Add(actor);
        return actor;
    }

    public IActor actor {
        get {
            Contract.Assume(actors.Count == 1);
            return actors.First();
        }
        set {
            add_actor(value);
        }
    }
    

    protected static units.parts.actions.Action.Pool<units.parts.actions.Action> pool { get; } = 
        new Pool<Action>();

    public string marker = "";

    public string get_marker() {
        return marker;
    }

    
    public Action() {
        root_action = this;
    }


    public Action add_marker(string in_marker) {
        marker = in_marker;
        return this;
    }
    

    public virtual void seize_needed_actors_recursive() {
        foreach(IActor seized_actor in actors) {
            if (seized_actor.current_action != null) {
                runner.mark_action_as_finishing(seized_actor.current_action.get_root_action());
            }
            seized_actor.current_action = this;
        }
    }



    public virtual void init_children() {
        
    }
    public virtual void init_actors() {
        
    }

    

    public virtual void set_root_action(Action in_root_action) {
        root_action = in_root_action;
        runner = root_action.runner;
    }

    public bool is_root() {
        return root_action == this;
    }
    public void attach_to_parent(Action in_parent) {
        parent_action = in_parent;
        set_root_action(in_parent.get_root_action());
    }

    public Action start_as_root(Action_runner action_runner) {
        Contract.Requires(action_runner != null);
        runner = action_runner;
        action_runner.start_root_action(this);
        return this;
    }

    public Action add_finish_notifyer(System.Action<Action> callback) {
        this.on_finished = callback;
        return this;
    }

   

    public virtual void update() {
        Contract.Requires(
            !is_reset,
            "can't update a deleted action"
        );
    }

    public bool superceded_as_root { get; private set; }
    public void mark_as_superceded() {
        root_action.superceded_as_root = true;
    }
 
    protected virtual void mark_as_completed() {
        if (completed) {
            return;
        }
        completed = true;

        if (parent_action != null) {
            parent_action.on_child_completed(this);
        }
        else {
            runner.on_root_action_completed(this);
            on_finished?.Invoke(this);
        }
    }
    
    protected void mark_as_not_completed() {
        if (!completed) {
            return;
        }

        Contract.Assume(completed, "should have reached its goal in order to lose it");
        completed = false;
        Contract.Assume(parent_action != null);
        parent_action.mark_as_not_completed();
    }

    public virtual void on_child_completed(Action child) {
        
    }
    
    

    public virtual void notify_actors_about_finishing() {
        foreach (IActor actor in actors) {
            actor.on_lacking_action();
        }
    }



    public void discard_whole_tree() {
        runner.mark_action_as_finishing(root_action);
    }






    public virtual void restore_state() {
        Log.info(String.Format("{0}: restore_state", GetType()));
    }

    public virtual void restore_state_recursive() {
        restore_state();
    }
    
    public virtual void init_state_recursive() {
        init_actors();
    }

    public virtual void init_children_recursive() {
        init_children();
    }

    public virtual void reset_recursive() {
        reset();
    }
    
    public virtual void finish() {
        restore_state();
        reset();
    }


    public virtual void free_actors_recursive() {
        foreach (IActor actor in actors) {
            if (actor.current_action == this) {
                actor.current_action = null;
            }
        }
    }


    public Action get_root_action() {
        if (this.parent_action != null) {
            return this.parent_action.get_root_action();
        }
        return this;
    }


    public virtual void reset() {
        Contract.Assert(!is_reset, "an action should be reset only once");
        actors.Clear();
        superceded_actions.Clear();
        parent_action = null;
        root_action = this;
        completed = false;
        marker = "";
        superceded_as_root = false;
        
        pool.return_to_pool(this);
    }

    public bool is_reset {
        get {
            return (
                    (parent_action == null) &&
                    (completed == false) &&
                    (is_free_in_pool == true)
                );
        }
    }

    
}



}