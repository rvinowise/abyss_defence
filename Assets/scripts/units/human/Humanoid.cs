using System;
using System.Collections;
using System.Collections.Generic;
using geometry2d;
using UnityEngine;
using rvinowise;
using rvinowise.rvi.contracts;
using rvinowise.units.control;
using rvinowise.units.control.human;
using rvinowise.units.parts.actions;
using rvinowise.units.parts.head;
using rvinowise.units.parts.humanoid;
using rvinowise.units.parts.limbs.arms;
using rvinowise.units.parts.limbs.arms.actions;
using rvinowise.units.parts.tools;
using rvinowise.units.parts.transport;
using rvinowise.units.parts.weapons;
using rvinowise.units.parts.weapons.guns;
using units.human.actions;
using UnityEngine.UIElements;
using Action = rvinowise.units.parts.actions.Action;
using Arm_controller = rvinowise.units.parts.limbs.arms.humanoid.Arm_controller;

namespace rvinowise.units.humanoid {

using parts;
using rvinowise.units.parts.limbs;

[RequireComponent(typeof(PolygonCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class Humanoid: 
    Turning_element,
    IPerform_actions
{
    
    /* parameters from the editor */
    public Sprite head_sprite;
    public init.Arms_initializer arms_initializer;
    
    

    /* parts of the human*/
    private Arm_controller arm_controller;
    private Head head;
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
    protected virtual void Awake()
    {
        base.Awake();
        init_components();
        init_parts();

        //animator.SetBool("open_hand", true);
    }

    private void init_components() {
        animator = GetComponent<Animator>();
        if (animator){
            //animator.SetTrigger("reload_pistol");
            animator.enabled = false;
        }
    }

    private void init_parts() {
        init_baggage();
        
        init_body_parts();
        init_intelligence();
    }

    private void init_baggage() {
        baggage = parts.humanoid.Baggage.create();
        put_tools_into_baggage(baggage);
        
        baggage.transform.parent = transform;
        //baggage.transform.localPosition = new Vector2(0.10f, -0.23f);
        baggage.transform.localRotation = Directions.degrees_to_quaternion(0f);
        baggage.transform.localPosition = new Vector2(0f, 0f);
        

        var sprite_renderer = baggage.game_object.add_component<SpriteRenderer>();
        sprite_renderer.sprite = Resources.Load<Sprite>("guns/pistol/pistol");
    }

    private void put_tools_into_baggage(Baggage baggage) {
        Tool pistol1 = Component_creator.instantiate_stashed(
            "objects/guns/desert_eagle/desert_eagle" 
        ).GetComponent<Tool>(); 
        
        Tool pistol2 = Component_creator.instantiate_stashed(
            pistol1
        ).GetComponent<Tool>();
        
        Tool break_sawedoff1 = Component_creator.instantiate_stashed(
            "objects/guns/break_sawedoff/break_sawedoff" 
        ).GetComponent<Tool>();
        
        Tool break_sawedoff2 = Component_creator.instantiate_stashed(
            break_sawedoff1
        ).GetComponent<Tool>();
        
        Tool pump_shotgun = Component_creator.instantiate_stashed(
            "objects/guns/pump_shotgun/pump_shotgun" 
        ).GetComponent<Tool>();

        Tool ak47 = Component_creator.instantiate_stashed(
            "objects/guns/ak47/ak47" 
        ).GetComponent<Tool>();
        
        
        baggage.items = new List<Tool> {
            pistol1,
            pistol2,
            break_sawedoff1,
            break_sawedoff2,
            pump_shotgun,
            ak47
        };

        Ammunition desert_eagle_magazine = Component_creator.instantiate(
            "objects/guns/desert_eagle/magazine" 
        ).GetComponent<Magazine>();
        baggage.insert_ammo_for_gun(pistol1.GetComponent<Desert_eagle>(), desert_eagle_magazine);
        
        //baggage.insert_ammo_for_gun(pump_shotgun.GetComponent<Pump_shotgun>(), desert_eagle_magazine);
    }

    private void init_body_parts() {
        
        head = init.Head.init(this, Head.create());

        legs = new Legs(this.gameObject);
        
        arm_controller = arms_initializer.init(
            new Arm_controller(this, legs),
            baggage,
            ui.input.Input.instance.cursor.center.transform
            //this.transform
        );
        start_default_activity();
    }

    private void start_default_activity() {
        arm_controller.left_arm.start_idle_action();
        arm_controller.right_arm.start_idle_action();
    }

    private void init_intelligence() {
        intelligence = new control.human.Player(transform);
        intelligence.transporter = this.legs;
        intelligence.arm_controller = arm_controller;
        intelligence.baggage = baggage;
        intelligence.sensory_organ = head;
        
        /*action_tree = Idle_vigilant_body.create(
            arm_controller.left_arm,
            arm_controller.right_arm,
            ui.input.Input.instance.cursor.center.transform,
            legs
        );*/
        
        
    }

    private Action last_action_test;
    void FixedUpdate() {
        intelligence.update();
        
        head.update();
        legs.update();
        

        if (current_action != last_action_test) {
            bool test = true;
        }
        last_action_test = current_action ;

        if (!animator.enabled) {
            current_action?.update();
            arm_controller.update();
        }
        
        //Debug.Log("deltaTime="+Time.deltaTime);
    }

    /* invoked from the animation (in keyframes).
    main_tool means the one in the right hand in the animation clip (can be flipped in the game) */
    /*void change_main_tool_animation(string in_animation) {
        Arm arm = arm_controller.right_arm;
        Contract.Requires(
            arm.held_tool != null,
            "must hold a tool to change its animation"
        );
        switch (in_animation) {
            case "sideview":
                arm.held_tool.animator.SetBool(in_animation, true);
                break;
            case "slide":
                arm.held_tool.animator.SetTrigger(in_animation);
                break;
        }
        
    }*/

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