using System;
using System.Collections;
using System.Collections.Generic;
using geometry2d;
using UnityEngine;
using rvinowise;
using rvinowise.rvi.contracts;
using rvinowise.units.control;
using rvinowise.units.parts.head;
using rvinowise.units.parts.limbs.arms.humanoid;
using rvinowise.units.parts.transport;
using rvinowise.units.parts.weapons;
using rvinowise.units.parts.weapons.guns.Pistol;
using UnityEngine.UIElements;

namespace rvinowise.units.human {

using parts;


[RequireComponent(typeof(PolygonCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(User_of_equipment))]
[RequireComponent(typeof(SpriteRenderer))]
public class Human: 
    MonoBehaviour 
{
    public Sprite head_sprite;
    
    private Intelligence intelligence;
    private User_of_equipment user_of_equipment;
    private Head head;
    private Arm_controller arms;
    private parts.humanoid.Baggage baggage;
    
    private SpriteRenderer sprite_renderer;
    
    protected virtual void Awake()
    {
        init_components();
        init_parts();
        
    }

    private void init_components() {
        user_of_equipment = GetComponent<User_of_equipment>();
        sprite_renderer = GetComponent<SpriteRenderer>();
        
    }

    private void init_parts() {
        init_baggage();
        init_body_parts();

        init_intelligence();
    }

    private void init_baggage() {
        baggage = new parts.humanoid.Baggage {
            items = new List<Gun> {
                new Pistol(),
                new Pistol()
            }
        };
        baggage.transform.parent = transform;
        baggage.transform.localRotation = Directions.degrees_to_quaternion(30f);
        baggage.transform.localPosition = new Vector2(0.10f, -0.23f);
//        baggage.transform.localRotation = Directions.degrees_to_quaternion(50f);
//        baggage.transform.localPosition = new Vector2(0.4f, 0f);
        

        baggage.game_object.AddComponent<SpriteRenderer>();
        var sprite_renderer = baggage.game_object.GetComponent<SpriteRenderer>();
        sprite_renderer.sprite = Resources.Load<Sprite>("guns/pistol/pistol");
        //Resources.Load<Sprite>("/guns/pistol/pistol");
    }

    private void init_body_parts() {
        sprite_renderer.sprite = Resources.Load<Sprite>("human/body");
        head = init.Head.init(this, new Head());
        arms = init.Arms.init(
            user_of_equipment.add_equipment_controller<parts.limbs.arms.humanoid.Arm_controller>()
        );
        arms.left_arm.baggage = arms.right_arm.baggage = baggage;
        arms.left_arm.idle_target = arms.right_arm.idle_target =
            ui.input.Input.instance.cursor.center.transform;
        //user_of_equipment.weaponry = arms;

    }

    private void init_intelligence() {
        intelligence = new Player_control(transform, user_of_equipment);
        intelligence.transporter = create_transporter();
        ((Player_control) intelligence).arm_controller = arms;
        intelligence.baggage = baggage;
        intelligence.sensory_organ = head;
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
    }
    
    

}
}