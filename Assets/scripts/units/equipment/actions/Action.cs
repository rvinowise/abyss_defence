using System;
using System.Collections.Generic;
using System.Diagnostics;
using rvinowise.debug;
using rvinowise.contracts;
using rvinowise.unity.units.control;
using rvinowise.unity.units.parts.limbs.arms.actions;
using UnityEngine.Animations;
using Debug = UnityEngine.Debug;
using rvinowise.unity.units.humanoid;

namespace rvinowise.unity.units.parts.actions {

public abstract partial class Action:
    IAction
{
    protected Action parent_action;

    protected Action root_action;

    
    
    
    public bool reached_goal { private set; get; }
    private bool is_free_in_pool;
    private Action superceded_action;
    public Intelligence notification_receiver;

    public virtual void on_child_reached_goal(Action in_sender_child) {
        Contract.Assert(false, "should be overridden in Parental actions");
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

    
    public virtual void start_as_root() {
        init_children_recursive();
        set_root_action(this);
        seize_needed_actors_recursive();
        stop_actions_which_use_same_actors_recursive();
        init_state_recursive();
    }

    public abstract void seize_needed_actors_recursive();
    

    public virtual void stop_actions_which_use_same_actors_recursive() {
        if (
            (superceded_action != null)&&(!superceded_action.is_reset)
        ) {
            superceded_action.discard_whole_tree();
        }
    }


    public virtual void init_children() {
        
    }
    public virtual void init_actors() {
        
    }

    

    public virtual void set_root_action(Action in_root_action) {
        root_action = in_root_action;
    }
    public void attach_to_parent(Action in_parent) {
        parent_action = in_parent;
    }

    

   

    public virtual void update() {
        Contract.Requires(
            !is_reset,
            "can't update a deleted action"
        );
    }
    

 
    protected void mark_as_reached_goal() {
        
        if (reached_goal) {
            return;
        }
        reached_goal = true;

        if (parent_action!=null) {
            parent_action.on_child_reached_goal(this);
        }
        else {
            finish_as_root_action();
        }
    }

    private void finish_as_root_action() {
        Contract.Requires(root_action == this, "this function is invoked from root");
        
        restore_state_recursive();
        free_actors_recursive();
        notify_intelligence_about_finishing();
        ensure_actors_have_next_action();
        reset_recursive();
    }
    
    private void discard_as_root_action() {
        Contract.Requires(root_action == this, "this function is invoked from root");
        
        restore_state_recursive();
        free_actors_recursive();
        ensure_actors_have_next_action();
        reset_recursive();
    }

    public void notify_intelligence_about_finishing() {
        Contract.Assert(notification_receiver != null);
        notification_receiver.on_action_finished(this);
    }

    public abstract void notify_actors_about_finishing();
    

    protected void mark_as_has_not_reached_goal() {
        if (!reached_goal) {
            return;
        }

        Contract.Assume(reached_goal, "should have reached its goal in order to lose it");
        reached_goal = false;
        Contract.Assume(parent_action != null);
        parent_action.mark_as_has_not_reached_goal();
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

    

    public void discard_whole_tree() {
        root_action.discard_as_root_action();
    }


    public abstract void ensure_actors_have_next_action();

    public abstract void free_actors_recursive();


    public Action get_root_action() {
        if (this.parent_action != null) {
            return this.parent_action.get_root_action();
        }
        return this;
    }


    public virtual void reset() {
        Contract.Assert(!is_reset, "an action should be reset only once");
        parent_action = null;
        root_action = this;
        reached_goal = false;
        marker = "";
        
        pool.return_to_pool(this);
    }

    public bool is_reset {
        get {
            return (
                    (parent_action == null) &&
                    (reached_goal == false) &&
                    (is_free_in_pool == true)
                );
        }
    }

    
}



}