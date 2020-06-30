using System;
using System.Collections.Generic;
using System.Diagnostics;
using rvinowise.debug;
using rvinowise.rvi.contracts;
using rvinowise.units.parts.limbs.arms.actions;
using UnityEngine.Animations;
using Debug = UnityEngine.Debug;

namespace rvinowise.units.parts.actions {

public abstract partial class Action {
    #region debug
    static int debug_count = 0;
    #endregion

    protected Action_parent parent_action { get; set; }
    
    public bool finished { private set; get; }

    protected static units.parts.actions.Action.Pool<units.parts.actions.Action> pool { get; } = 
        new Pool<Action>();

   
    /*protected Action(Action_sequence_builder in_parent_sequence) {
        parent_sequence = in_parent_sequence;
        debug_count++;
        Debug.Log("Action qty= "+debug_count);
        Log.info(string.Format("Action is created. type= {1}, qty= {0}", 
            debug_count,
            this.GetType()
        ));
    }*/
    public Action() {
        debug_count++;
        Debug.Log("Action qty= " + debug_count);
        Log.info(string.Format("Action is created. type= {1}, qty= {0}",
            debug_count,
            this.GetType()
        ));
    }

    public virtual void attach_to_parent(Action_parent in_action_parent) {
        parent_action = in_action_parent;
        
    }

    
    

    ~Action() {
        debug_count--;
        Debug.Log("Action qty= "+debug_count);
        Log.info(string.Format("Action is destroyed. type= {1}, qty= {0}", 
            debug_count,
            this.GetType()
        ));
    }

    public virtual void update() {}
    
    protected void reached_goal() {
        #region debug;
        Log.info($"{GetType()} finish");
        #endregion

        Contract.Assume(!finished, "no need to finish twice");
        Contract.Requires(parent_action != null, "root parent action can never end");
        
        finished = true;
        parent_action.on_child_reached_goal(this);
    }

    public virtual void finish() {
        Contract.Requires(
            parent_action != null,
            "the root parent action cannot finish"
        );
        restore_state();
        discard();
    }

    public virtual void discard() {
        reset();
        pool.return_to_pool(this);
    }

    protected void reset() {
        parent_action = null;
        finished = false;
    }
    
    public virtual void init_state() {
        Log.info(($"{GetType()} Action is started."));
    }

    public virtual void restore_state() {
        Log.info($"{GetType()} Action is ended. ");
    }
}



}