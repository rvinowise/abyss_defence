using System;
using System.Collections.Generic;
using System.Linq;
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
    ,IChild_of_group
{
    public ALeg limb;
    public Creeping_leg_group creeping_leg_group;
    
    public float segment1_swinging_degree;
    public float segment2_swinging_degree;
    public float segment1_hitting_degree;
    public float segment2_hitting_degree;
    
    public Transform damage_point;
    public Animated_attacker_attacked_area attacked_area;

    private System.Action<actions.Action> intelligence_on_shielded;


    private void Awake() {
        if (!attacked_area) {
            attacked_area = GetComponent<Animated_attacker_attacked_area>();
        }
    }

    #region IWeaponry

    public float damaging_radius = 0.5f;
    public bool is_weapon_ready_for_target(Transform target) {
        // var target_collider = target.GetComponent<Collider2D>();
        // if (target_collider != null) {
        //     return target_collider.OverlapPoint(damage_point.transform.position);
        // }
        if (damage_point.distance_to(target.position) < damaging_radius) {
            return true;
        }
        return false;
    }

    public IEnumerable<Damage_receiver> get_targets() {
        //return Enumerable.Empty<Damage_receiver>();\
        return attacked_area.reacheble_damageable_enemies;
    }

    public float get_reaching_distance() {
        return limb.segment1.length + limb.segment2.length;
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
        .start_as_root(actor.action_runner);
    }

    #endregion IWeaponry
    
    
    
    #region IActor

    public Actor actor { get; set; }

    public void on_lacking_action() {
        
    }

    #endregion

    

}

}
