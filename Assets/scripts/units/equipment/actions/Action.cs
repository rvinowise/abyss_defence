using System;
using System.Collections.Generic;
using System.Diagnostics;
using rvinowise.debug;
using rvinowise.rvi.contracts;
using rvinowise.units.parts.limbs.arms.actions;
using units.equipment.arms.Arm.actions;
using UnityEngine.Animations;
using Debug = UnityEngine.Debug;

namespace rvinowise.units.parts.actions {

public abstract partial class Action {
    #region debug
    static int debug_count = 0;
    #endregion

    public Action next_action;
    protected Action_parent parent_action { get; set; }

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

    protected void reset() {
        parent_action = null;
        next_action = null;
    }
    

    protected void transition_to_next_action() {
        #region debug;
        Log.info($"{GetType()} transition_to_next_action");
        #endregion
        if (next_action != null) {
            transition_to(next_action);
        } else {
            finish_parent_action();
            
        }
    }

    protected void transition_to(Action in_next_action) {
        Contract.Requires(
            in_next_action!=null, 
            "if no next action, another function is used"
        );
        Contract.Requires(
            parent_action!=null, 
            "changes of actions can occur only as part of a higher-level action"
        );
        restore_state();
        parent_action.start_next_child_action(in_next_action);
        discard();
    }

    private void finish_parent_action() {
        Contract.Requires(
            parent_action!=null, 
            "parent action must exist to finish it"
        );
        restore_state();
        parent_action.transition_to_next_action();
        discard();
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

    public virtual void init_state() {
        Log.info(($"{GetType()} Action is started."));
    }

    public virtual void restore_state() {
        Log.info($"{GetType()} Action is ended. ");
    }


    public void discard() {
        reset();
        pool.return_to_pool(this);
    }
}



}