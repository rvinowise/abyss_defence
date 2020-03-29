using System;
using System.Collections.Generic;

namespace rvinowise.units.parts.actions {

public abstract partial class Action {

    //protected virtual IDo_actions user { get; }
    protected abstract Action_tree action_tree { get; set; }
    
    public Action next;

    protected static units.parts.actions.Action.Pool<units.parts.actions.Action> pool { get; } = 
        new Pool<Action>();
    
    static int debug_count = 0;
    protected Action(Action_tree in_action_tree) {
        action_tree = in_action_tree;
        debug_count++;
        //Debug.Log("strategies qty: "+debug_count);
    }

    public Action() {
        
    }
    
    ~Action() {
        debug_count--;
        //Debug.Log("strategies qty: "+debug_count);
    }

    public virtual void update() {}
    public virtual void start() {}

    public virtual void end() {
        pool.return_to_pool(this);
    }

    protected void start_next() {
        action_tree.change(next);
    }

    public virtual void OnDrawGizmos() {
    }
}



}