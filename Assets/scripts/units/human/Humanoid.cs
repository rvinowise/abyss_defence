using System;
using System.Collections;
using System.Collections.Generic;
using rvinowise.unity.geometry2d;
using UnityEngine;
using rvinowise.unity.extensions;

using rvinowise;
using rvinowise.contracts;
using rvinowise.unity.units.control;
using rvinowise.unity.units.control.human;
using rvinowise.unity.units.parts.actions;
using rvinowise.unity.units.parts.head;
using rvinowise.unity.units.parts.humanoid;
using rvinowise.unity.units.parts.limbs.arms;
using rvinowise.unity.units.parts.limbs.arms.actions;
using rvinowise.unity.units.parts.tools;
using rvinowise.unity.units.parts.transport;
using rvinowise.unity.units.parts.weapons;
using rvinowise.unity.units.parts.weapons.guns;
using units.human.actions;
using UnityEngine.UIElements;
using Action = rvinowise.unity.units.parts.actions.Action;
using Arm_pair = rvinowise.unity.units.parts.limbs.arms.humanoid.Arm_pair;

namespace rvinowise.unity.units.humanoid {
    using global::units;
    using parts;
    using rvinowise.unity.extensions.attributes;
    using rvinowise.unity.units.parts.limbs;

[RequireComponent(typeof(PolygonCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class Humanoid: 
    rvinowise.unity.units.Unit,
    IFlippable_actor, IActor
{

    /* parts of the human*/
    public Arm_pair arm_pair;
    public Head head;
    [SerializeField]
    private units.parts.humanoid.Legs legs;
    public parts.humanoid.Baggage baggage;
    
    
    //private IChildren_groups_host user_of_equipment;
    
    
    /* components */
    private SpriteRenderer sprite_renderer;
    public Animator animator;
    
    
    #region IActor
    public Action current_action { set; get; }

    public void on_lacking_action() {
        
    }

    private Action_runner action_runner;
    public void init_for_runner(Action_runner action_runner) {
        this.action_runner = action_runner;
    }
    #endregion


    /* Humanoid itself */

    public void pick_up(Tool in_tool) {
        if (in_tool is Ammunition ammo) {
            baggage.change_ammo_qty(ammo.compatibility, ammo.rounds_qty);
        }
    }

    public void pick_up(Ammunition in_ammo) {
        baggage.change_ammo_qty(
            in_ammo.compatibility,
            in_ammo.rounds_qty
            );
    }
    protected override void Awake()
    {
        base.Awake();
        init_components();
    }

    private void init_components() {
        animator = GetComponent<Animator>();
        if (animator){
            animator.enabled = false;
        }
        arm_pair = GetComponent<Arm_pair>();
    }

    


    void FixedUpdate() {
        head.rotate_to_desired_direction();
    }

  
    
    /* [called_in_animation]
    void change_main_tool_animation(AnimationEvent in_event) {
        
        switch (in_event.stringParameter) {
            case "sideview":
               
                break;
            case "slide":
                
                break;
            
            case "opened":
                break;
        }
        
    } */

    /* IFlippable_actor interface */
    public void flip_for_animation(bool in_flipped) {
        if (in_flipped != is_flipped()) {
            float scale = Mathf.Abs(transform.localScale.x);
            if (in_flipped) {
                transform.localScale = new Vector3(scale, -scale, 1);
            } else {
                transform.localScale = new Vector3(scale, scale, 1);
            }
            
            switch_tools();
        }
    }
    public bool is_flipped() {
        return transform.localScale.y < 0;
    }

    public void restore_after_flipping() {
        Contract.Requires(is_flipped(), "restoring after flipping is only makes sense if flipped");
        arm_pair.switch_arms_angles();
        stop_arms();
        flip_for_animation(false);
        //Debug.Break();
    }
    
    private void stop_arms() {
        arm_pair.left_arm.shoulder.current_rotation_inertia = 0;
        arm_pair.left_arm.segment1.current_rotation_inertia = 0;
        arm_pair.left_arm.segment2.current_rotation_inertia = 0;
        arm_pair.left_arm.segment3.current_rotation_inertia = 0;
        arm_pair.right_arm.shoulder.current_rotation_inertia = 0;
        arm_pair.right_arm.segment1.current_rotation_inertia = 0;
        arm_pair.right_arm.segment2.current_rotation_inertia = 0;
        arm_pair.right_arm.segment3.current_rotation_inertia = 0;
    }
    
    private void switch_tools() {
        arm_pair.switch_tools();
        
    }

   
}
}