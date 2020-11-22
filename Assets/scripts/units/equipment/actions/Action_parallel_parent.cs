using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using rvinowise.unity.extensions;

using rvinowise;
using rvinowise.unity.geometry2d;
using rvinowise.rvi.contracts;


namespace rvinowise.unity.units.parts.actions {

public class Action_parallel_parent: 
    Action_parent
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
    
    protected override void add_child(Action in_child) {
        child_actions.Add(in_child);
        in_child.attach_to_parent(this);
    }
    
    protected override void add_children(params Action[] in_children) {
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

    /* IPerform_actions interface */    
    public override IEnumerable<Action> current_active_children {
        get {
            return child_actions;
        }
    }
    public override IEnumerable<Action> queued_children {
        get { return new Action[]{}; }
    }
    
    
    //private Queue<Action> child_actions = new Queue<Action>();
    

    private void discard_child_actions() {
        foreach (Action child_action in child_actions) {
            child_action.discard_from_queue();
        }
        child_actions.Clear();
    }

    public override void init_state() {
        foreach (Action child in current_active_children) {
            child.init_state();
        }
    }

    public override void update() {
        base.update();
        Contract.Requires(
            child_actions.Count > 0,
            "Action parent must have a child to execute"
        );
        foreach (Action child_action in child_actions) {
            child_action.update();
        }
        if (all_children_are_finished()) {
            mark_as_reached_goal();
        }
    }

    /*public override void on_child_reached_goal(Action in_sender_child) {
        if (all_children_are_finished()) {
            mark_as_reached_goal();
        }
    }*/

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

    public override void discard_during_execution() {
        foreach (Action active_child in child_actions) {
            active_child.discard_during_execution();
        }
        child_actions.Clear();
        base.discard_during_execution();
    }

    public override void discard_from_queue() {
        foreach (Action active_child in child_actions) {
            active_child.discard_from_queue();
        }
        child_actions.Clear();
        base.discard_from_queue();
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