using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rvinowise;
using rvinowise.rvi.contracts;
using rvinowise.units.control;
using rvinowise.units.parts;
using rvinowise.units.control;
using rvinowise.units.parts.head;
using rvinowise.units.parts.limbs.arms.humanoid;
using rvinowise.units.parts.transport;
using UnityEngine.UIElements;


namespace rvinowise.units.human {

using parts;


[RequireComponent(typeof(PolygonCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(User_of_equipment))]
public class Human: 
    MonoBehaviour {
    public Sprite head_sprite;
    
    private Intelligence intelligence;
    private User_of_equipment user_of_equipment;
    private Head head;
    private Arm_controller arms;
    
    private SpriteRenderer sprite_renderer;
    
    protected virtual void Awake()
    {
        
        
        
        init_components();
        init_body();
        
    }

    private void init_components() {
        user_of_equipment = GetComponent<User_of_equipment>();
        Contract.Requires(user_of_equipment != null);
        sprite_renderer = GetComponent<SpriteRenderer>();
        
    }

    private void init_body() {
        intelligence = new Player_control(transform, user_of_equipment);
        intelligence.transporter = create_transporter();
        
        this.sprite_renderer.sprite = Resources.Load<Sprite>("human/body");
        head = init.Head.init(this, new Head());
        arms = init.Arms.init(
            new parts.limbs.arms.humanoid.Arm_controller(user_of_equipment)
        );
    }


    protected ITransporter create_transporter() {
        return user_of_equipment.
            add_equipment_controller<units.parts.humanoid.Legs>();
    }
    protected IWeaponry create_weaponry() {
        return new Arm_controller(user_of_equipment);
    }
    //protected abstract IWeaponry get_weaponry();

    void Update() {
        intelligence.update();
        head.update();
        head.desired_direction = (rvi.Input.mouse_world_position() - head.position).to_quaternion();
    }
    
    

}
}