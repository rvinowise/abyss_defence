using System;
using System.Collections.Generic;
using rvinowise.contracts;
using UnityEngine;


namespace rvinowise.unity.actions {

public abstract partial class Action
{
    internal Action parent_action;

    private Action root_action;


    public bool is_started { private set; get; } = false;
    public bool is_completed { private set; get; }
    public bool is_free_in_pool;
    public System.Action<Action> on_completed;
    public System.Action on_completed_without_parameter;
    protected Action_runner runner;

    //private readonly List<Action> superceded_actions = new List<Action>();
    public Guid id = Guid.NewGuid();

    

    protected static Object_pool<Action> object_pool { get; } = 
        new Object_pool<Action>();

    public string marker = "";

    protected Action() {
        root_action = this;
    }


    public Action add_marker(string in_marker) {
        marker = in_marker;
        return this;
    }


    protected virtual void on_start_execution() {
        rvinowise.contracts.Contract.Assert(is_started==false, "a started action is started again without finishing");
        is_started = true;
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
        action_runner.prepare_root_action_for_start(this);
        return this;
    }

    public Action set_on_completed(System.Action<Action> callback) {
        this.on_completed = callback;
        return this;
    }
    public Action set_on_completed(System.Action callback) {
        this.on_completed_without_parameter = callback;
        return this;
    }

   

    public virtual void update() {
        Contract.Requires(
            !is_reset,
            "can't update a deleted action"
        );
        Contract.Requires(
            is_started,
            "can't update a not started action"
        );
    }

    public bool superceded_as_root { get; private set; }
    public void mark_as_superceded() {
        root_action.superceded_as_root = true;
    }
 
    protected void mark_as_completed() {
        if (is_completed) {
            return;
        }
        is_completed = true;

        if (parent_action != null) {
            parent_action.on_child_completed(this);
        }
        else {
            runner.on_root_action_completed(this);
        }
    }
    
    protected void mark_as_not_completed() {
        if (!is_completed) {
            return;
        }

        Contract.Assume(is_completed, "should have reached its goal in order to lose it");
        is_completed = false;
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

    protected virtual void restore_state() {
        System.Diagnostics.Contracts.Contract.Assert(is_started, $"action {get_explanation()} restores the state, but it wasn't started");
    }


    public Action get_root_action() {
        if (this.parent_action != null) {
            return this.parent_action.get_root_action();
        }
        return this;
    }


    public virtual void reset() {
        rvinowise.contracts.Contract.Assert(!is_reset, $"an action {get_explanation()} was reset, but is being reset again");

        //superceded_actions.Clear();
        parent_action = null;
        root_action = this;
        is_completed = false;
        is_started = false;
        marker = "";
        superceded_as_root = false;
        on_completed = null;
        on_completed_without_parameter = null;
        
        object_pool.return_to_pool(this);
    }

    public bool is_reset {
        get {
            return parent_action == null &&
                   is_completed == false &&
                   is_free_in_pool;
        }
    }

    public virtual string get_actors_names() {
        return "";
    }
    public virtual string get_explanation() {
#if OPTIMIZED
        return "";
#else
        if (parent_action != null)
            return $"{parent_action.get_explanation()} -> {GetType().Name}[{get_actors_names()}]{id:N}";
        
        return $"{GetType().Name}_[{get_actors_names()}]{id:N}";
#endif
    }
    


}



}