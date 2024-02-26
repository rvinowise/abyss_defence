using System;
using System.Collections.Generic;
using UnityEngine;

using rvinowise.contracts;
using rvinowise.unity.actions;
using rvinowise.unity.extensions;
using rvinowise.unity.geometry2d;
using Action = rvinowise.unity.actions.Action;


namespace rvinowise.unity {

public class Shielding_limb:
    MonoBehaviour
    ,IDefender
    ,IChild_of_group
    ,IRunning_actions
{
    public ALeg shielding_limb;
    public Creeping_leg_group creeping_leg_group;
    public float segment1_shielding_degree;
    public float segment2_shielding_degree;
    public Span defended_span;
    
    private Action_runner action_runner;

    private System.Action intelligence_on_shielded;
    
    
    #region IDefender
    public void start_defence(Transform danger, System.Action on_completed) {
        intelligence_on_shielded = on_completed;
        place_shield_before_danger(danger);
    }
    
    public void finish_defence(System.Action on_completed) {
        Creeping_leg_partakes_in_moving.create(shielding_limb).start_as_root(action_runner);
        on_completed?.Invoke();
    }
    
    protected void place_shield_before_danger(Transform danger) {
        creeping_leg_group.ensure_leg_raised(shielding_limb);
        Limb2_reach_relative_directions.create_assuming_left_limb(
            shielding_limb,
            segment1_shielding_degree,
            segment2_shielding_degree,
            creeping_leg_group.transform
        ).set_on_completed(on_leg_is_shielding)
        .start_as_root(action_runner);
    }

    protected void on_leg_is_shielding() {
        Idle.create(
            shielding_limb
        ).start_as_root(action_runner);
        
        intelligence_on_shielded();
    }
    
    #endregion IDefender
    
    
    
    #region IRunning_actions

    public void init_for_runner(Action_runner action_runner) {
        this.action_runner = action_runner;
    }
    
    #endregion

    void OnCollisionEnter2D(Collision2D collision) {
        
        if (collision.get_damaging_projectile() is Projectile damaging_projectile ) {
            var contact = collision.GetContact(0);
            damaging_projectile.stop_at_position(contact.point);
        }
    }
    
    private void OnTriggerEnter2D(Collider2D collider2d) {
        if (collider2d.get_damaging_projectile() is Projectile damaging_projectile ) {
            damaging_projectile.stop_at_position(damaging_projectile.transform.position);
        }
    }
    
    private void OnDrawGizmosSelected() {
        float line_length = 0.3f;
        Gizmos.color = Color.green;
        
        var min_rotation = transform.rotation * Directions.degrees_to_quaternion(defended_span.min);
        var max_rotation = transform.rotation * Directions.degrees_to_quaternion(defended_span.max);
        Gizmos.DrawLine(creeping_leg_group.transform.position, creeping_leg_group.transform.position+min_rotation * Vector2.right * line_length);
        Gizmos.DrawLine(creeping_leg_group.transform.position, creeping_leg_group.transform.position+max_rotation * Vector2.right * line_length);
    }

}

}