using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using rvinowise.unity.extensions;

using rvinowise;
using rvinowise.unity.geometry2d;
using rvinowise.contracts;
using UnityEngine.Video;

namespace rvinowise.unity.units.parts.actions {

public class Action_sequential_parent: 
    Action,
    IAction_parent
{

    public Queue<Action> queued_child_actions = new Queue<Action>();


    
    public static Action_sequential_parent create(
        IPerform_actions in_actor,
        params Action[] children
    ) {
        Action_sequential_parent action = 
            (Action_sequential_parent)pool.get(
                typeof(Action_sequential_parent)
            );
        action.set_actor(in_actor);
        foreach (Action child in children) {
            action.add_child(child);
        }
        return action;
    }

    public override void set_root_action(Action in_root_action) {
        base.set_root_action(in_root_action);
        current_child_action?.set_root_action(in_root_action);
        foreach (Action child in queued_child_actions) {
            child.set_root_action(in_root_action);
        }
    }

    public void add_child(Action in_child) {
        if (current_child_action == null) {
            current_child_action = in_child;
        }
        else {
            queued_child_actions.Enqueue(in_child);
        }
        in_child.attach_to_parent(this);
    }

    public void add_children(params Action[] in_children) {
        foreach(var child in in_children) {
            child.attach_to_parent(this);
        }
        current_child_action = in_children.First();
        for (int i_child = 1; i_child < in_children.Length; i_child++) {
            queued_child_actions.Enqueue(in_children[i_child]);
        }
    }
    
    
    public Action current_child_action { private set; get; }


    
    
    public override void start_execution() {
        init_state();
        current_child_action.start_execution();
    }

    public override void update() {
        base.update();
        if (current_child_action == null) {
            bool test = true;
        }
        Contract.Requires(
            current_child_action != null,
            "Action parent must have a child to execute"
        );
    }

    public override void on_child_reached_goal(Action in_sender_child) {
        Contract.Requires(
            in_sender_child == current_child_action, 
            "only one child of a parallel parent can be executed at a time"
        );
        
        if (queued_child_actions.Any()) {
            current_child_action.finish_at_completion();
            Action next_action = queued_child_actions.Dequeue();
            start_next_child_action(next_action);
        } else {
            mark_as_reached_goal();
        }
        
    }
    
    public void start_next_child_action(Action next_action) {
        current_child_action = next_action;
        current_child_action.start_execution();
    }
    
    public override void start_default_action() {
        base.start_default_action();
        current_child_action.start_default_action();
    }

    private void replace_this_by(Action in_action) {
        //parent_action.
    }
    
    
    public override void finish_at_completion() {
        Contract.Requires(
            !queued_child_actions.Any(),
            "sequential parent can only finish when no child actions left in the queue"
        );
        Contract.Requires(
            current_child_action.reached_goal,
            "sequential action can only finish when its current child has reached goal"
        );
        
        base.finish_at_completion();
    }

    public override void restore_state_and_delete() {
        if (marker.StartsWith("root sequence")) {
            var test = true;
        }
        current_child_action.restore_state_and_delete();
        current_child_action = null;
        delete_queued_child_actions();
        base.restore_state_and_delete();
    }
    
    public override void delete() {
        current_child_action?.delete();
        current_child_action = null;
        delete_queued_child_actions();
        base.delete();
    }
    
    private void delete_queued_child_actions() {
        
        foreach(Action child_action in queued_child_actions) {
            child_action.delete();
        }
        queued_child_actions.Clear();
    }
    
}
}