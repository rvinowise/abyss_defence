using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rvinowise;
using rvinowise.units.parts.limbs.arms.actions;
using rvinowise.units.parts.strategy;
using Action = rvinowise.units.parts.limbs.arms.actions.Action;


namespace rvinowise.units.parts.limbs.arms.strategy {

public class Idle_vigilant: Action {

    private Transform target;

    public static Idle_vigilant create(
        Action_tree in_action_tree, Transform in_target
    ) {
        Idle_vigilant action = (Idle_vigilant)pool.get(typeof(Idle_vigilant), in_action_tree);
        action.target = in_target;
        return action;
    }
    public Idle_vigilant(Action_tree in_action_tree, Transform in_target): base(in_action_tree) {
        target = in_target;
    }
    
    public Idle_vigilant() {
        
    }
    
    public override void start() {
        //this.arm.hand.animator.SetInteger("Gesture", Hand_gesture.Relaxed.Value);
    }
    public override void update() {
        
        var direction_to_mouse = arm.upper_arm.transform.quaternion_to(target.position);
        arm.upper_arm.desired_direction =
            arm.upper_arm.desired_idle_direction * direction_to_mouse;
        
        arm.forearm.desired_direction =
            arm.forearm.desired_idle_direction * direction_to_mouse;
        arm.hand.desired_direction =
            arm.hand.desired_idle_direction * direction_to_mouse;
    }
}
}