using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rvinowise;
using geometry2d;
using rvinowise.rvi.contracts;
using rvinowise.units.parts.actions;
using Action = rvinowise.units.parts.actions.Action;

namespace units.equipment.arms.Arm.actions {

public class Action_parent: 
    Action,
    Action_sequence_builder
{

    public Action current_child_action {
        set {
            discard_child_action_chain();
            start_next_child_action(value);
        }
        get { return _current_child_action; }
    }
    private Action _current_child_action;

    public Action new_next_child {
        get { return _last_added_child.next_action; }
        set {
            _last_added_child.next_action = value;
            _last_added_child = value;
        }
    }
    private Action _last_added_child;

    
    
    public Action start_new_child(Action in_action) {
        discard_child_action_chain();
        start_next_child_action(in_action);

        return in_action;
    }
    
    public Action add_next_child(Action in_action) {
        _last_added_child.next_action = in_action;
        _last_added_child = in_action;

        return in_action;
    }
    
    //private Queue<Action> child_actions = new Queue<Action>();
    
    
    public override void attach_to_parent(Action_parent in_action_parent) {
        base.attach_to_parent(in_action_parent);
        discard_child_action_chain();
    }

    public void start_next_child_action(Action next_action) {
        _current_child_action = next_action;
        _last_added_child = _current_child_action;
        _current_child_action.init_state();

    }

    private void discard_child_action_chain() {
        Action child_action = _current_child_action;
        while (child_action != null) {
            Action next_child_action = child_action.next_action;
            child_action.discard();
            child_action = next_child_action;
        }
        _current_child_action = null;
        _last_added_child = null;
    }


    public override void update() {
        Contract.Requires(
            current_child_action != null,
            "Action parent must have a child to execute"
        );
        current_child_action.update();
    }

    
}
}