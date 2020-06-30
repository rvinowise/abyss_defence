using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rvinowise;
using geometry2d;
using rvinowise.rvi.contracts;


namespace rvinowise.units.parts.actions {

public class Action_parallel_parent: 
    Action_parent
{

    public static Action_parallel_parent create(
        Action_parent in_action_parent
    ) {
        var action = (Action_parallel_parent)pool.get(typeof(Action_parallel_parent), in_action_parent);
        
        return action;
    }
    
    public List<Action> child_actions = new List<Action>();

    
    public Action start_new_child(Action in_action) {
        discard_child_actions();

        return in_action;
    }
    
    public Action add_child(Action in_action) {
        child_actions.Add(in_action);
        return in_action;
    }
    
    //private Queue<Action> child_actions = new Queue<Action>();
    

    private void discard_child_actions() {
        foreach (Action child_action in child_actions) {
            child_action.discard();
        }
        child_actions.Clear();
    }


    public override void update() {
        Contract.Requires(
            child_actions.Count > 0,
            "Action parent must have a child to execute"
        );
        foreach (Action child_action in child_actions) {
            child_action.update();
        }
    }

    public override void on_child_reached_goal(Action in_sender_child) {
        if (all_children_are_finished()) {
            reached_goal();
        }
    }

    public override void finish() {
        foreach (Action child_action in child_actions) {
            child_action.finish();
        }
        base.finish();
    }

    private bool all_children_are_finished() {
        foreach (Action child_action in child_actions) {
            if (!child_action.finished) {
                return false;
            }
        }
        return true;
    }

    
}
}