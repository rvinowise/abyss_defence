using System.Collections.Generic;
using rvinowise.contracts;
using UnityEngine;


namespace rvinowise.unity.units.parts.actions {

public abstract partial class Action
{
    private Action parent_action;

    private Action root_action;

    public bool completed { private set; get; }
    private bool is_free_in_pool;
    private System.Action<Action> on_finished;
    protected Action_runner runner;

    private readonly List<Action> superceded_actions = new List<Action>();
    

    

    protected static units.parts.actions.Action.Pool<units.parts.actions.Action> pool { get; } = 
        new Pool<Action>();

    public string marker = "";

    protected Action() {
        root_action = this;
    }


    public Action add_marker(string in_marker) {
        marker = in_marker;
        return this;
    }


    protected virtual void on_start_execution() {}
    

    

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
 
    protected void mark_as_completed() {
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
    
    

    public void discard_whole_tree() {
        runner.mark_action_as_finishing(root_action);
    }


    public virtual void restore_state_recursive() {
        restore_state();
    }

    public virtual void start_execution_recursive() {
        on_start_execution();
    }

   
    public virtual void reset_recursive() {
        reset();
    }

    public virtual void free_actors_recursive() { }
    public virtual void notify_actors_about_finishing_recursive() { }
    public virtual void seize_needed_actors_recursive() { }

    protected virtual void restore_state() { }


    public Action get_root_action() {
        if (this.parent_action != null) {
            return this.parent_action.get_root_action();
        }
        return this;
    }


    public virtual void reset() {
        Contract.Assert(!is_reset, $"an action {this.marker} was reset, but is being reset again");
        if (is_reset)
            Debug.LogError("error");
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
            return parent_action == null &&
                   completed == false &&
                   is_free_in_pool;
        }
    }

    


}



}