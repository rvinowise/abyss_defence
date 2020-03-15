using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rvinowise;


namespace rvinowise.units.parts.limbs.arms.strategy {

public class Idle_vigilant:Strategy {

    private Transform target;

    public Idle_vigilant(Arm arm, Transform in_target): base(arm) {
        target = in_target;
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