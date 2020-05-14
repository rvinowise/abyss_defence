using System;
using System.Collections;
using System.Collections.Generic;
using geometry2d;
using UnityEngine;
using rvinowise;
using rvinowise.rvi.contracts;
using rvinowise.units.control;
using rvinowise.units.control.human;
using rvinowise.units.parts.head;
using rvinowise.units.parts.humanoid;
using rvinowise.units.parts.limbs.arms;
using rvinowise.units.parts.limbs.arms.actions;
using rvinowise.units.parts.tools;
using rvinowise.units.parts.transport;
using rvinowise.units.parts.weapons.guns;
using UnityEngine.UIElements;
using Arm_controller = rvinowise.units.parts.limbs.arms.humanoid.Arm_controller;

namespace rvinowise.units.human {

using parts;


[RequireComponent(typeof(PolygonCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class Human: 
    MonoBehaviour 
{
    
    /* parameters from the editor */
    public Sprite head_sprite;
    public init.Arms_initializer arms_initializer;
    
    

    /* parts of the human*/
    private Arm_controller arms;
    private Head head;
    private units.parts.humanoid.Legs legs;
    private parts.humanoid.Baggage baggage;
    
    
    private IChildren_groups_host user_of_equipment;
    private units.control.human.Human intelligence;

    
    /* components */
    private SpriteRenderer sprite_renderer;
    private Animator animator;
    
    protected virtual void Awake()
    {
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
        baggage.transform.localPosition = new Vector2(0.10f, -0.23f);
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

        Magazine desert_eagle_magazine = Component_creator.instantiate(
            "objects/guns/desert_eagle/magazine" 
        ).GetComponent<Magazine>();
        baggage.insert_ammo_for_gun(pistol1.GetComponent<Desert_eagle>(), desert_eagle_magazine);
    }

    private void init_body_parts() {
        
        head = init.Head.init(this, Head.create());

        legs = new Legs(this.gameObject);
        
        arms = arms_initializer.init(
            new Arm_controller(this, legs),
            baggage,
            ui.input.Input.instance.cursor.center.transform
        );
        
    }

    private void init_intelligence() {
        intelligence = new control.human.Player(transform);
        intelligence.transporter = this.legs;
        intelligence.arm_controller = arms;
        intelligence.baggage = baggage;
        intelligence.sensory_organ = head;
        
        //test
        /*Arm arm = ((control.human.Player)intelligence).get_selected_arm();
        arm.action.current_child_action = Grab_tool.create(
            arm.action, arm, baggage, baggage.items[0] 
        );
        
        arm.action.new_next_child = Idle_vigilant_only_arm.create(
            arm.action,
            arm,
            arm.idle_target, 
            arm.controller.transporter
        );*/
        //endtest
    }


    void FixedUpdate() {
        intelligence.update();
        
        head.update();
        legs.update();
        arms.update();
    }

    void LateUpdate() {
        //private void OnAnimatorIK(int layerIndex) {
        
    }

    void change_tool_animation(string in_animation) {
        Arm arm = arms.right_arm;
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
        
    }
    
}
}