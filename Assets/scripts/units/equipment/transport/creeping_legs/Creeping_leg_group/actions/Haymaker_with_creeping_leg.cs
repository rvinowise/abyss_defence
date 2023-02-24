using System;
using System.Collections;
using System.Collections.Generic;
using rvinowise.unity.geometry2d;
using UnityEngine;
using rvinowise.unity.extensions;

using rvinowise;
using rvinowise.unity.units.control;
using rvinowise.unity.units.parts.actions;
using rvinowise.unity.units.parts.limbs.actions;
using rvinowise.unity.units.parts.limbs.arms.actions;
using rvinowise.unity.units.parts.limbs.creeping_legs;
using rvinowise.unity.units.parts.transport;
using units;
using Action = rvinowise.unity.units.parts.actions.Action;


namespace rvinowise.unity.units.parts.limbs.arms.actions {

public class Haymaker_with_creeping_leg: Action_sequential_parent {
    
    private static readonly int animation_haymaker = Animator.StringToHash("Base Layer.hitting_with_haymaker");

    protected Animator animator;
    private Creeping_leg_group leg_group;
    private Transform target;
    private Leg2 attacking_leg;
    
    public static Haymaker_with_creeping_leg create(
        Animator in_animator,
        Creeping_leg_group leg_group,
        Transform in_target
    ) {
        var action = (Haymaker_with_creeping_leg)pool.get(typeof(Haymaker_with_creeping_leg));
        action.animator = in_animator;
        action.leg_group = leg_group;
        action.target = in_target;
        action.attacking_leg = leg_group.left_front_leg as Leg2;
        action.init_child_actions();
        return action;
    }

    private void init_child_actions() {
        
        add_child(
            Limb2_reach_relative_directions.create_assuming_left_limb(
                attacking_leg,
                60,
                -90
            )
        );
        add_child(
            Play_recorded_animation.create(
                animator,
                animation_haymaker
            )
        );

    }
    

    public override void init_actors() {
        base.init_actors();
        leg_group.ensure_leg_raised(attacking_leg);
    }

  
}
}