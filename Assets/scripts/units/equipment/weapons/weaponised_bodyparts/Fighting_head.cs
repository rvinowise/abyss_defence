using System;
using System.Collections.Generic;
using rvinowise.unity.actions;
using rvinowise.unity.extensions;
using rvinowise.unity.extensions.attributes;
using UnityEngine;
using Action = rvinowise.unity.actions.Action;


namespace rvinowise.unity {
public class Fighting_head :
    MonoBehaviour,
    //Attacker_child_of_group,
    //IAttacker,
    ISensory_organ
{

    public Animated_attacker animated_attacker;

    public Turning_element turning_element;

    public void pay_attention_to_target(Transform target) {
        var direction_to_target = transform.quaternion_to(target.position);
        turning_element.set_target_rotation(direction_to_target);
        turning_element.rotate_to_desired_direction();
    }

    public bool is_focused_on_target() {
        return true;
    }

    // #region IWeaponry interface
    // public override bool is_weapon_ready_for_target(Transform target) {
    //     return animated_attacker.is_weapon_ready_for_target(target);
    // }
    //
    // public override float get_reaching_distance() {
    //     return animated_attacker.get_reaching_distance();
    // }
    //
    // public override void attack(Transform target, System.Action on_completed = null) {
    //     animated_attacker.attack(target, on_completed);
    //
    // }
    //
    // #endregion

    

    #region IActor
     
    public Actor actor { get; set; }

    public void on_lacking_action() {
        Idle.create(actor).start_as_root(actor.action_runner);
    }

    #endregion

}

}