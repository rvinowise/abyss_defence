using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using rvinowise.unity.extensions;

using rvinowise;
using rvinowise.unity.geometry2d;
using rvinowise.contracts;


namespace rvinowise.unity.units.parts.actions {

public class Action_parallel_parent: 
    Action,
    IAction_parent
{

    public static Action_parallel_parent create(
        IPerform_actions in_actor,
        params Action[] children
    ) {
        var action = (Action_parallel_parent)pool.get(
            typeof(Action_parallel_parent)
        );

        action.set_actor(in_actor);
        
        foreach (Action child in children) {
            action.add_child(child);
            
        }
        
        return action;
    }
    
    public void add_child(Action in_child) {
        child_actions.Add(in_child);
        in_child.attach_to_parent(this);
    }
    
    public void add_children(params Action[] in_children) {
        foreach (Action child in in_children) {
            child_actions.Add(child);
            child.attach_to_parent(this);
        }
    }
    
    
    public Action start_new_child(Action in_action) {
        discard_child_actions();

        return in_action;
    }
   
    public List<Action> child_actions = new List<Action>();


    public override void set_root_action(Action in_root_action) {
        base.set_root_action(in_root_action);
        foreach (Action child in child_actions) {
            child.set_root_action(in_root_action);
        }
    }
    
    private void discard_child_actions() {
        foreach (Action child_action in child_actions) {
            child_action.delete();
        }
        child_actions.Clear();
    }


    public override void start_execution() {
        init_state();
        foreach (Action child in child_actions) {
            child.start_execution();
        }
    }
   

    public override void update() {
        base.update();
        Contract.Requires(
            child_actions.Count > 0,
            "Action parent must have a child to execute"
        );
    }

    public override void on_child_reached_goal(Action in_sender_child) {
        if (all_children_are_finished()) {
            mark_as_reached_goal();
        }
    }
    
    public override void start_default_action() {
        base.start_default_action();
        foreach(var child in child_actions) {
            child.start_default_action();
        }
    }

    public void discard_while_executing() {
        foreach (Action active_child in child_actions) {
            active_child.detach_from_parent();
        }
    }
    public override void finish_at_completion() {
        foreach (Action child in child_actions) {
            Contract.Requires(
                child.reached_goal == true,
                "all child actions should be finished when the parent is finished"
                );
        }
        
        foreach (Action child_action in child_actions) {
            child_action.finish_at_completion();
        }
        child_actions.Clear();
        base.finish_at_completion();
    }

    public override void restore_state_and_delete() {
        foreach (Action active_child in child_actions) {
            if (active_child.marked_for_deletion) {
                active_child.restore_state_and_delete();
            }
            else {
                active_child.detach_from_parent();
            }
        }
        child_actions.Clear();
        base.restore_state_and_delete();
    }

    public override void delete() {
        foreach (Action active_child in child_actions) {
            active_child.delete();
        }
        child_actions.Clear();
        base.delete();
    }

    private bool all_children_are_finished() {
        foreach (Action child_action in child_actions) {
            if (!child_action.reached_goal) {
                return false;
            }
        }
        return true;
    }

    
}
}