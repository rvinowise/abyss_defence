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
using Arm_controller = rvinowise.unity.units.parts.limbs.arms.humanoid.Arm_controller;

namespace rvinowise.unity.units.humanoid {

using parts;
using rvinowise.unity.units.parts.limbs;

[RequireComponent(typeof(PolygonCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class Humanoid: 
    Turning_element,
    IPerform_actions
{
    
    /* parameters from the editor */
    public init.Arms_initializer arms_initializer;
    
    

    /* parts of the human*/
    private Arm_controller arm_controller;
    public Head head;
    [SerializeField]
    private units.parts.humanoid.Legs legs;
    public parts.humanoid.Baggage baggage;
    
    
    private IChildren_groups_host user_of_equipment;
    private units.control.human.Human intelligence;
    
    
    /* components */
    private SpriteRenderer sprite_renderer;
    public Animator animator;
    
    
    /* IPerformActions interface */
    public Action current_action {
        set {
            _current_action?.discard_whole_tree();
            _current_action = value;
        }
        get { return _current_action; }
    }
    private Action _current_action;
    
    public void set_root_action(Action in_action) {
        current_action = in_action;
        current_action.start_as_root();
    }
    
    
    /* Humanoid itself */
    protected override void Awake()
    {
        base.Awake();
        init_components();
        init_parts();

        //animator.SetBool("open_hand", true);
    }

    protected virtual void Start() {
        start_default_activity();        
    }

    private void init_components() {
        animator = GetComponent<Animator>();
        if (animator){
            //animator.SetTrigger("reload_pistol");
            animator.enabled = false;
        }
        arm_controller = GetComponent<Arm_controller>();
    }

    private void init_parts() {
        init_baggage();
        
        init_body_parts();
        init_intelligence();
    }

    private void init_baggage() {
        //put_tools_into_baggage(baggage);
    }

    private void init_body_parts() {
        
        init.Head.init(head);
        
        
    }

    private void start_default_activity() {
        arm_controller.left_arm.start_idle_action();
        arm_controller.right_arm.start_idle_action();
    }

    private void init_intelligence() {
        intelligence.arm_controller = arm_controller;
    }

    private Action last_action_test;
    void FixedUpdate() {
        
        head.rotate_to_desired_direction();
        legs.update();
        
        last_action_test = current_action ;

        if (!animator.enabled) {
            current_action?.update();
            arm_controller.update();
        }
    }

  

    void change_main_tool_animation(AnimationEvent in_event) {
        Arm arm = arm_controller.right_arm;
        Contract.Requires(
            arm.held_tool != null,
            "must hold a tool to change its animation"
        );
        Tool held_tool = arm.held_tool;
        switch (in_event.stringParameter) {
            case "sideview":
                held_tool.animator.SetBool(in_event.stringParameter, Convert.ToBoolean(in_event.intParameter));
                held_tool.transform.localScale = new Vector3(1,1,1);
                break;
            case "slide":
                held_tool.animator.SetTrigger(in_event.stringParameter);
                break;
        }
        
    }
    
    /* invoked from the animation (in keyframes).*/
    void apply_ammunition_to_gun(AnimationEvent in_event) {
        Arm gun_arm = arm_controller.right_arm;
        Arm ammo_arm = arm_controller.other_arm(gun_arm);
        Contract.Requires(
            gun_arm.held_tool is Gun,
            "must hold a tool to reload it"
        );
        Contract.Requires(
            ammo_arm.held_tool is Ammunition,
            "must hold a tool to reload it"
        );

        if (
            (gun_arm.held_tool is Pistol pistol)&&
            (ammo_arm.held_tool is Magazine magazine)
        )
        {
            pistol.insert_magazine(magazine);
        } else {
            Gun gun = gun_arm.held_tool as Gun;
            Ammunition ammo = ammo_arm.held_tool as Ammunition;
            gun.apply_ammunition(ammo);
        }
    }

    
}
}