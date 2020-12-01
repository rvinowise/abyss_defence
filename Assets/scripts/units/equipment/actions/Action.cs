using System;
using System.Collections.Generic;
using System.Diagnostics;
using rvinowise.debug;
using rvinowise.contracts;
using rvinowise.unity.units.parts.limbs.arms.actions;
using UnityEngine.Animations;
using Debug = UnityEngine.Debug;

namespace rvinowise.unity.units.parts.actions {

public abstract partial class Action {
    #region debug
    static int debug_count = 0;
    #endregion

    protected Action_parent parent_action { get; set; }
    protected Action root_action;
    protected IPerform_actions actor;

    protected void set_actor(IPerform_actions in_actor) {
        actor = in_actor;
    }
    
    
    public bool reached_goal { private set; get; }
    private bool is_free_in_pool = false;

    protected static units.parts.actions.Action.Pool<units.parts.actions.Action> pool { get; } = 
        new Pool<Action>();

   

    public static Action new_root_action(
        System.Type action_type,
        params Object[] param    
    ) {
        var action = (Take_tool_from_bag)pool.get(typeof(Take_tool_from_bag));
        return action;
    }
    
    public Action() {
        debug_count++;
        Log.info(string.Format("Action is created. type= {1}, qty= {0}",
            debug_count,
            this.GetType()
        ));
        root_action = this;
    }

    public void start_as_root() {
        set_root_action(this);
        init_state();
    }

    public virtual void set_root_action(Action in_root_action) {
        root_action = in_root_action;
    }
    public virtual void attach_to_parent(Action_parent in_parent) {
        parent_action = in_parent;
        //root_action = in_parent.root_action;
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
        if (reached_goal) {
            return;
        }
        #region debug;
        Log.info($"{GetType()} finish");
        #endregion
        
        Contract.Requires(parent_action != null, "root parent action can never end");
        
        reached_goal = true;
        parent_action.on_child_reached_goal(this);
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
    
    public virtual void init_state() {
        Log.info(($"{GetType()} Action is started."));
        prepare_actor_for_execution();
    }
    
    private void prepare_actor_for_execution() {
        if (actor_is_used_in_another_action())
        {
            actor.current_action.discard_whole_tree();
            actor.current_action = null;
        }

        bool actor_is_used_in_another_action() {
            return 
                (actor.current_action != null)&&
                (actor.current_action != root_action);
        }
    }

    public virtual void restore_state() {
        Log.info($"{GetType()} Action is ended. ");
    }
    
    
    public virtual void finish_at_completion() {
        Contract.Requires(
            parent_action != null,
            "the root parent action cannot finish"
        );
        discard_during_execution();
    }

    public virtual void discard_during_execution() {
        restore_state();
        
        discard_from_queue();
    }

    public virtual void discard_from_queue() {
        reset();
        pool.return_to_pool(this);
    }


    public void discard_whole_tree() {
        if (this_is_root_action()) {
            discard_during_execution();
        } else {
            parent_action.discard_whole_tree();
        }

        bool this_is_root_action() {
            return this.parent_action == null;
        }
    }


    protected virtual void reset() {
        parent_action = null;
        root_action = this;
        reached_goal = false;
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