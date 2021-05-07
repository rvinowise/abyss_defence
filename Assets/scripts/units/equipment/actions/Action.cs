using System;
using System.Collections.Generic;
using System.Diagnostics;
using rvinowise.debug;
using rvinowise.contracts;
using rvinowise.unity.units.parts.limbs.arms.actions;
using UnityEngine.Animations;
using Debug = UnityEngine.Debug;
using rvinowise.unity.units.humanoid;

namespace rvinowise.unity.units.parts.actions {

public abstract partial class Action:
    IAction
{
    #region debug
    static int debug_count = 0;
    #endregion

    protected Action parent_action { 
        get{return _parent_action;} 
        set{
            _parent_action = value;
            if (_parent_action == null) {
                var test = true;
            }
        } 
    }
    Action _parent_action;

    protected Action root_action;
    protected internal IPerform_actions actor;

    
    
    
    public bool reached_goal { private set; get; }
    private bool is_free_in_pool = false;

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
        debug_count++;
        Log.info(string.Format("Action is created. type= {1}, qty= {0}",
            debug_count,
            this.GetType()
        ));
        root_action = this;
    }

    public Action add_marker(string in_marker) {
        marker = in_marker;
        return this;
    }

    protected void set_actor(IPerform_actions in_actor) {
        actor = in_actor;
        
    }
    public void start_as_root() {
        set_root_action(this);
        start_execution();
    }

    public void add_to_parent(IAction_parent in_parent) {
        in_parent.add_child(this);
        set_root_action(in_parent.get_root_action());
    }

    public virtual void init_state() {
        
    }

    public virtual void start_execution() {
        if (actor != null) {
            change_action_of_actor();
        }
        init_state();
    }

    public virtual void set_root_action(Action in_root_action) {
        root_action = in_root_action;
    }
    public void attach_to_parent(Action in_parent) {
        parent_action = in_parent;
        root_action = in_parent.root_action;
    }

    

    ~Action() {
        debug_count--;
        //Debug.Log("Action qty= "+debug_count);
        Log.info(string.Format("Action is destroyed. type= {1}, qty= {0}", 
            debug_count,
            this.GetType()
        ));
    }

    public virtual void update() {
        Contract.Requires(
            !is_deleted,
            "can't update a deleted action"
        );
    }
    

 
    protected void mark_as_reached_goal() {
        if (this.marker.StartsWith("second tier")) {
            var test = true;
        }
        if (this is Pull_tool_out_of_bag) {
            var test = true;
        }
        if (reached_goal) {
            return;
        }
        #region debug;
        Log.info($"{GetType()} finish");
        #endregion
        
        if (parent_action!=null) {
            notify_parent_about_finishing();
        }
        else {
            start_default_action();

        }
    }

    private void notify_parent_about_finishing() {
        Contract.Requires(parent_action != null, "root parent action can never end");
        reached_goal = true;    
        parent_action.on_child_reached_goal(this);
    }

    public virtual void start_default_action() {
        actor.start_default_action();
        actor.current_action = actor.get_default_action();
    }

    protected void mark_as_has_not_reached_goal() {
        if (!reached_goal) {
            return;
        }
        #region debug;
        Log.info($"{GetType()} lost its goal, isn't finished again");
        #endregion
        Contract.Assume(reached_goal, "should have reached its goal in order to lose it");
        reached_goal = false;
    }
    
    
    
    public void change_action_of_actor() {
        Log.info(($"{GetType()} Action is started."));
        //if (actor_is_used_in_another_action())
        if (actor.current_action != null)
        {
            Contract.Requires(
                actor.get_root_action() != this.root_action,
                "child actions within one root action shouldn't clash for one actor"
            );
            actor.current_action.discard_whole_tree();
        }
        actor.current_action = this;
    }

   

    public virtual void restore_state() {
        Log.info($"{GetType()} Action is ended. ");
    }
    
    
    public virtual void finish_at_completion() {
        Contract.Requires(
            parent_action != null,
            "the root parent action cannot finish"
        );
        restore_state_and_delete();
    }

    public virtual void restore_state_and_delete() {
        restore_state();
        if (actor != null) {
            actor.current_action = null;
        }
        
        delete();
    }
    
    public 

    public void detach_from_parent() {
        this.parent_action = null;
        this.set_root_action(this);
    }

    public virtual void delete() {
        reset();
        pool.return_to_pool(this);
    }


    public void discard_whole_tree() {
        Action root_action = mark_actions_for_deletion();
        root_action.restore_state_and_delete();
    }

    private Action mark_actions_for_deletion() {
        if (this.parent_action != null) {
            marked_for_deletion = true;
            return this.parent_action.mark_actions_for_deletion();
        }
        return this;
    }

    public bool marked_for_deletion { get; set; } = false;

    public Action get_root_action() {
        if (this.parent_action != null) {
            return this.parent_action.get_root_action();
        }
        return this;
    }


    protected void reset() {
        parent_action = null;
        root_action = this;
        reached_goal = false;
        marked_for_deletion = false;
    }

    public bool is_deleted {
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