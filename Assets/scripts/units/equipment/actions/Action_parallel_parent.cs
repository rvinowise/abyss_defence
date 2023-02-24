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


namespace rvinowise.unity.units.parts.actions {

public class Action_parallel_parent: 
    Action,
    IAction_parent
{

    public static Action_parallel_parent create(
        params Action[] children
    ) {
        var action = (Action_parallel_parent)pool.get(
            typeof(Action_parallel_parent)
        );

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
    
    public override void finish() {
        foreach (Action child_action in child_actions) {
            child_action.finish();
        }
        child_actions.Clear();
        base.finish();
    }


    public override void reset() {
        child_actions.Clear();
        base.reset();
    }

    private bool all_children_are_completed() {
        foreach (Action child_action in child_actions) {
            if (!child_action.completed) {
                return false;
            }
        }
        return true;
    }

    public override void free_actors_recursive() {
        base.free_actors_recursive();
        foreach (Action child in child_actions) {
            child.free_actors_recursive();
        }
    }
    
    public override void seize_needed_actors_recursive() {
        foreach (Action child in child_actions) {
            child.seize_needed_actors_recursive();
        }
        base.seize_needed_actors_recursive();
    }

    public override void init_state_recursive() {
        init_actors();
        foreach (Action child in child_actions) {
            child.init_state_recursive();
        }
    }
    public override void init_children_recursive() {
        init_children();
        foreach (Action child in child_actions) {
            child.init_children_recursive();
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

    public override void notify_actors_about_finishing() {
        foreach (Action child in child_actions) {
            child.notify_actors_about_finishing();
        }
    }
}
}