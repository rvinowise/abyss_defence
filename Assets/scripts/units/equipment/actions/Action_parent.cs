using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rvinowise.unity.extensions;

using rvinowise;
using rvinowise.unity.geometry2d;


namespace rvinowise.unity.units.parts.actions {

public abstract class Action_parent: 
    Action {

    public abstract IEnumerable<Action> current_active_children { get; }
    public abstract IEnumerable<Action> queued_children { get; }
    
    public override void set_root_action(Action in_root_action) {
        base.set_root_action(in_root_action);
        foreach (Action child in current_active_children) {
            child.set_root_action(in_root_action);
        }
        foreach (Action child in queued_children) {
            child.set_root_action(in_root_action);
        }
    }
    
    public virtual void on_child_reached_goal(Action in_sender_child){}

    protected abstract void add_child(Action in_action);

    protected virtual void add_children(params Action[] in_children) { }

    //public abstract Action create(params Action[] children);


}
}