using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using rvinowise;
using geometry2d;
using rvinowise.rvi.contracts;

namespace rvinowise.units.parts.actions {

public class Action_sequential_parent: 
    Action_parent,
    Action_sequence_builder
{

    public Queue<Action> child_actions = new Queue<Action>();

    public Action current_child_action_setter {
        set {
            discard_child_action_chain();
            start_next_child_action(value);
        }
        //get { return current_child_action; }
    }
    public Action current_child_action { private set; get; }

    public Action new_next_child {
        get { return child_actions.Last(); }
        set {
            child_actions.Enqueue(value);
        }
    }
    private Action last_added_child {
        get { return child_actions.Last(); }
    }

    
    
    public Action start_new_child(Action in_action) {
        discard_child_action_chain();
        start_next_child_action(in_action);

        return in_action;
    }
    
    public Action add_next_child(Action in_action) {
        child_actions.Enqueue(in_action);

        return in_action;
    }
    
    //private Queue<Action> child_actions = new Queue<Action>();
    
    
    


    public override void update() {
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
        current_child_action.finish();
        
        
        if (child_actions.Any()) {
            Action next_action = child_actions.Dequeue();
            start_next_child_action(next_action);
        } else {
            reached_goal();
        }
        
    }
    
    public void start_next_child_action(Action next_action) {
        current_child_action = next_action;
        current_child_action.init_state();

    }
    
    
    public override void finish() {
        Contract.Requires(
            !child_actions.Any(),
            "sequential parent can only finish when no child actions left in the queue"
        );
        Contract.Requires(
            current_child_action.finished,
            "parent action can only finish when its current child is finished"
        );
        
        base.finish();
    }

    public override void discard() {
        discard_child_action_chain();
        base.discard();
    }
    
    private void discard_child_action_chain() {
        foreach(Action child_action in child_actions) {
            child_action.discard();
        }
        current_child_action = null;
    }
    
}
}