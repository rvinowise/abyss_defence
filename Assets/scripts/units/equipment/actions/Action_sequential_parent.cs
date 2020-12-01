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
    Action_parent,
    Action_sequence_builder
{

    public Queue<Action> queued_child_actions = new Queue<Action>();


    public static Action_sequential_parent create(params Action[] children) {
        Action_sequential_parent action = 
            (Action_sequential_parent)pool.get(
                typeof(Action_sequential_parent)
            );

        foreach (Action child in children) {
            action.add_child(child);
        }
        return action;
    }

    protected void construct(params System.Object[] children) {
        foreach (Action child in children) {
            add_child(child);
        }
    }
    
    protected override void add_child(Action in_child) {
        if (current_child_action == null) {
            current_child_action = in_child;
        }
        else {
            queued_child_actions.Enqueue(in_child);
        }
        in_child.attach_to_parent(this);
    }

    protected override void add_children(params Action[] in_children) {
        current_child_action = in_children.First();
        current_child_action.attach_to_parent(this);
        for (int i_child = 1; i_child < in_children.Length; i_child++) {
            queued_child_actions.Enqueue(in_children[i_child]);
            in_children[i_child].attach_to_parent(this);
        }
    }
    
    public Action current_child_action_setter {
        set {
            discard_queued_child_actions();
            start_next_child_action(value);
        }
        //get { return current_child_action; }
    }
    public Action current_child_action { private set; get; }

    public Action new_next_child {
        get { return queued_child_actions.Last(); }
        set {
            queued_child_actions.Enqueue(value);
        }
    }
    private Action last_added_child {
        get { return queued_child_actions.Last(); }
    }

    public override IEnumerable<Action> current_active_children {
        get {
            return new Action[]{current_child_action};
        }
    }
    public override IEnumerable<Action> queued_children {
        get { return queued_child_actions; }
    }
    
    
    
    public Action start_new_child(Action in_action) {
        discard_queued_child_actions();
        start_next_child_action(in_action);

        return in_action;
    }
    
    public Action add_next_child(Action in_action) {
        queued_child_actions.Enqueue(in_action);

        return in_action;
    }
    
    //private Queue<Action> child_actions = new Queue<Action>();


    public override void init_state() {
        current_child_action.init_state();
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
        current_child_action.update();
    }

    public override void on_child_reached_goal(Action in_sender_child) {
        Contract.Requires(
            in_sender_child == current_child_action, 
            "only one child of a parallel parent can be executed at a time"
        );
        
        if (queued_child_actions.Any()) {
            current_child_action.finish_at_completion();
            Action next_action = queued_child_actions.Dequeue();
            //if (queued_child_actions.Any()) {
                start_next_child_action(next_action);
            /*} else {
                replace_this_by(next_action);
            }*/
        } else {
            mark_as_reached_goal();
        }
        
    }
    
    public void start_next_child_action(Action next_action) {
        current_child_action = next_action;
        current_child_action.init_state();

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

    public override void discard_during_execution() {
        if (current_child_action == null) {
            bool test = true;
        }
        current_child_action.discard_during_execution();
        current_child_action = null;
        discard_queued_child_actions();
        base.discard_during_execution();
    }
    
    public override void discard_from_queue() {
        current_child_action?.discard_from_queue();
        current_child_action = null;
        discard_queued_child_actions();
        base.discard_from_queue();
    }
    
    private void discard_queued_child_actions() {
        
        foreach(Action child_action in queued_child_actions) {
            child_action.discard_from_queue();
        }
        queued_child_actions.Clear();
    }
    
}
}