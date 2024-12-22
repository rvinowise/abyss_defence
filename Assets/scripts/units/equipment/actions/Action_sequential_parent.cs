using System.Collections.Generic;
using System.Linq;
using rvinowise.contracts;
using rvinowise.debug;
using UnityEngine;


namespace rvinowise.unity.actions {

public class Action_sequential_parent :
    Action,
    IAction_parent 
{

    public Queue<Action> queued_child_actions = new Queue<Action>();



    public static Action_sequential_parent create(
        params Action[] children
    ) {
        Action_sequential_parent action =
            (Action_sequential_parent) object_pool.get(
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
        if (current_child_action == null) {
            current_child_action = in_children.First();
            for (int i_child = 1; i_child < in_children.Length; i_child++) {
                queued_child_actions.Enqueue(in_children[i_child]);
            }
        }
        else {
            foreach (var child_action in in_children) {
                queued_child_actions.Enqueue(child_action);
            }
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
            
            if (
                (next_child.is_reset)||
                (next_child.parent_action == null)
                )
            {
                Debug.LogError($"next_child {next_child.marker} of action {marker} is reset");
            }
            
            current_child_action = next_child;
        }
        else {
            mark_as_completed();
        }
    }

    private void replace_this_by(Action in_action) {
        //parent_action.
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


    public override void start_execution_recursive() {
        on_start_execution();
        if (current_child_action==null)
            Debug.LogError($"current_child_action of sequential_action [{this.marker}] is null");
        current_child_action.start_execution_recursive();
    }
    
    public override void restore_state_recursive() {
        if (current_child_action.is_started) {
            current_child_action.restore_state_recursive();
        }
        restore_state();
    }
    

    public override void reset_recursive() {
        current_child_action.reset_recursive();
        reset();
    }
    
    
    public override void free_actors_recursive() {
        current_child_action.free_actors_recursive();
    }
    

    public override void seize_needed_actors_recursive() {
        current_child_action.seize_needed_actors_recursive();
    }
    
    public override void notify_actors_about_finishing_recursive() {
        current_child_action.notify_actors_about_finishing_recursive();
    }
 
    public override string get_actors_names() {
        if (current_child_action != null)
            return current_child_action.get_actors_names();
        return "NO_CHILD";
    }
    
}
}