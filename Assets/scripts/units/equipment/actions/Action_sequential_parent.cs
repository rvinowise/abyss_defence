using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using rvinowise.unity.extensions;

using rvinowise;
using rvinowise.unity.geometry2d;
using rvinowise.contracts;
using rvinowise.debug;
using UnityEngine.Video;

namespace rvinowise.unity.units.parts.actions {

public class Action_sequential_parent :
    Action,
    IAction_parent {

    public Queue<Action> queued_child_actions = new Queue<Action>();



    public static Action_sequential_parent create(
        params Action[] children
    ) {
        Action_sequential_parent action =
            (Action_sequential_parent) pool.get(
                typeof(Action_sequential_parent)
            );
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
        foreach (var child in in_children) {
            child.attach_to_parent(this);
        }
        current_child_action = in_children.First();
        for (int i_child = 1; i_child < in_children.Length; i_child++) {
            queued_child_actions.Enqueue(in_children[i_child]);
        }
    }


    public Action current_child_action { private set; get; }
    

    public override void update() {
        base.update();
        Contract.Requires(
            current_child_action != null,
            "Action parent must have a child to execute"
        );
        current_child_action.update();
    }

    public override void on_child_completed(Action in_sender_child) {
        Contract.Requires(
            in_sender_child == current_child_action,
            "only one child of a sequential parent can be executed at a time"
        );

        if (queued_child_actions.Any()) {
            Action next_child = queued_child_actions.Dequeue();
            Contract.Assert(current_child_action != null);
            Contract.Assert(runner != null);
            runner.mark_action_as_finishing(current_child_action);
            runner.mark_action_as_starting(next_child);
            current_child_action = next_child;
        }
        else {
            mark_as_completed();
        }
    }

    private void replace_this_by(Action in_action) {
        //parent_action.
    }
    
    
    public override void finish() {
        current_child_action.finish();
        base.finish();
    }
    
    
    
    public override void reset() {
        current_child_action = null;
        reset_queued_child_actions();
        base.reset();
    }
    
    private void reset_queued_child_actions() {
        
        foreach(Action child_action in queued_child_actions) {
            child_action.reset();
        }
        queued_child_actions.Clear();
    }
    
    public override void free_actors_recursive() {
        current_child_action.free_actors_recursive();
    }
    

    public override void seize_needed_actors_recursive() {
        current_child_action.seize_needed_actors_recursive();
    }

    
    public override void restore_state_recursive() {
        current_child_action.restore_state_recursive();
        restore_state();
    }
    public override void init_state_recursive() {
        init_actors();
        current_child_action.init_state_recursive();
    }
    public override void init_children_recursive() {
        init_children();
        current_child_action.init_children_recursive();
    }

    public override void reset_recursive() {
        current_child_action.reset_recursive();
        reset();
    }
    
    public override void notify_actors_about_finishing() {
        current_child_action.notify_actors_about_finishing();
    }
    
}
}