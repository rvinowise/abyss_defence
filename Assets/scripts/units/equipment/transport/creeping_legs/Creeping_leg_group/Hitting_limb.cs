using System;
using System.Collections.Generic;
using UnityEngine;

using rvinowise.contracts;
using rvinowise.unity.actions;
using rvinowise.unity.extensions;
using rvinowise.unity.geometry2d;
using Action = rvinowise.unity.actions.Action;


namespace rvinowise.unity {

public class Hitting_limb:
    MonoBehaviour
    ,IAttacker
    ,IRunning_actions
    ,IChild_of_group
{
    public ALeg limb;
    public Creeping_leg_group creeping_leg_group;
    
    public float segment1_swinging_degree;
    public float segment2_swinging_degree;
    public float segment1_hitting_degree;
    public float segment2_hitting_degree;
    
    public Transform damage_point;
    
    private Action_runner action_runner;

    private System.Action<actions.Action> intelligence_on_shielded;
    
    
    #region IWeaponry

    public float damaging_radius = 0.5f;
    public bool can_reach(Transform target) {
        // var target_collider = target.GetComponent<Collider2D>();
        // if (target_collider != null) {
        //     return target_collider.OverlapPoint(damage_point.transform.position);
        // }
        if (damage_point.distance_to(target.position) < damaging_radius) {
            return true;
        }
        return false;
    }

    public void attack(Transform target, System.Action on_completed = null) {
        creeping_leg_group.ensure_leg_raised(limb);
        Action_sequential_parent.create(
            Limb2_reach_relative_directions.create_assuming_left_limb(
                limb,
                segment1_swinging_degree,
                segment2_swinging_degree,
                creeping_leg_group.transform
            ),
            Limb2_reach_relative_directions.create_assuming_left_limb(
                limb,
                segment1_hitting_degree,
                segment2_hitting_degree,
                creeping_leg_group.transform
            )
        ).set_on_completed(on_completed)
        .start_as_root(action_runner);
    }

    #endregion IWeaponry
    
    
    
    #region IRunning_actions

    public void init_for_runner(Action_runner action_runner) {
        this.action_runner = action_runner;
    }
    
    #endregion

    

}

}