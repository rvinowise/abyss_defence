using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rvinowise;
using rvinowise.rvi.contracts;
using rvinowise.units.parts.actions;
using rvinowise.units.parts.limbs.arms;
using rvinowise.units.parts.strategy;


namespace rvinowise.units.parts.limbs.arms.actions {

public class Action: parts.actions.Action {

    protected Arm arm {
        get { return arm_action_tree.arm; }
    }

    protected override parts.actions.Action_tree action_tree {
        get { return arm_action_tree; }
        set {
            Contract.Requires(value is arms.actions.Action_tree);
            arm_action_tree = value as arms.actions.Action_tree;
        }
    }
    //protected abstract override Pool<TAction> pool { get; }
    private arms.actions.Action_tree arm_action_tree;


    public Action(Action_tree in_arm_action_tree) : base(in_arm_action_tree) {
        arm_action_tree = in_arm_action_tree;
    }

    public Action() {
        
    }
    
    public override void update() {}
}
}