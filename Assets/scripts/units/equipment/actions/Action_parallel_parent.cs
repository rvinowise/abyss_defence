using System;
using System.Collections.Generic;
using System.Linq;
using rvinowise.contracts;


namespace rvinowise.unity.actions {

public class Action_parallel_parent: 
    Action,
    IAction_parent
{

    public static Action_parallel_parent create(
        params Action[] children
    ) {
        var action = (Action_parallel_parent)object_pool.get(
            typeof(Action_parallel_parent)
        );

        foreach (Action child in children) {
            action.add_child(child);
            
        }
        
        return action;
    }

    public static Action_parallel_parent create_from_actions(
        IEnumerable<Action> child_actions
    ) {
        var action = create();
        action.add_children(child_actions);
        return action;
    }
    
    
    public void add_child(Action in_child) {
        child_actions.Add(in_child);
        in_child.attach_to_parent(this);
    }
    
    public void add_children(params Action[] in_children) {
        foreach (Action child in in_children) {
            add_child(child);
        }
    }
    public void add_children(IEnumerable<Action> in_children) {
        foreach (Action child in in_children) {
            add_child(child);
        }
    }
    
   
    public List<Action> child_actions = new List<Action>();

    

    public override void set_root_action(Action in_root_action) {
        base.set_root_action(in_root_action);
        foreach (Action child in child_actions) {
            child.set_root_action(in_root_action);
        }
    }



    public override void update() {
        base.update();
        Contract.Requires(
            child_actions.Count > 0,
            "Action parent must have a child to execute"
        );
        foreach (Action child in child_actions) {
            child.update();
        }
        
    }

    public override void on_child_completed(Action child) {
        if (all_children_are_completed()) {
            mark_as_completed();
        }
    }
    



    public override void reset() {
        child_actions.Clear();
        base.reset();
    }

    private bool all_children_are_completed() {
        foreach (Action child_action in child_actions) {
            if (!child_action.is_completed) {
                return false;
            }
        }
        return true;
    }

    

    public override void start_execution_recursive() {
        on_start_execution();
        foreach (Action child in child_actions) {
            child.start_execution_recursive();
        }
    }
    
    public override void restore_state_recursive() {
        foreach (Action child in child_actions) {
            child.restore_state_recursive();
        }
        restore_state();
    }
    public override void reset_recursive() {
        foreach (Action child in child_actions) {
            child.reset_recursive();
        }
        reset();
    }

    
    public override void free_actors_recursive() {
        foreach (Action child in child_actions) {
            child.free_actors_recursive();
        }
    }
    
    public override void seize_needed_actors_recursive() {
        foreach (Action child in child_actions) {
            child.seize_needed_actors_recursive();
        }
    }
    public override void notify_actors_about_finishing_recursive() {
        foreach (Action child in child_actions) {
            child.notify_actors_about_finishing_recursive();
        }
    }
    
    public override string get_actors_names() {
        var names = new List<string>();
        if (child_actions.Any()) {
            foreach (var child in child_actions) {
                names.Add(child.get_actors_names());
            }
            return String.Join(", ", names);
        }
        return "NO_CHILDREN";
    }
}
}