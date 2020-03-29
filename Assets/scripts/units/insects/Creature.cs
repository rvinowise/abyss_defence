using System.Collections;
using System.Collections.Generic;
using System;
using rvinowise.units.parts;
using rvinowise.units.control;
using rvinowise.units.control.human;
using rvinowise.units.parts.transport;
using UnityEngine;

namespace rvinowise.units {

[RequireComponent(typeof(PolygonCollider2D))]
[RequireComponent(typeof(User_of_equipment))]
public abstract class Creature : MonoBehaviour {
    
    protected Divisible_body divisible_body;


    protected User_of_equipment user_of_equipment {
        get { return intelligence.user_of_equipment; }
    }
    
    Intelligence intelligence;

    
    protected virtual void Awake()
    {
        divisible_body = gameObject.GetComponent<Divisible_body>();
        
        intelligence = new Player(
            transform,
            GetComponent<User_of_equipment>()
        );

        if (divisible_body.needs_initialisation) {
            equip();
            divisible_body.needs_initialisation = false;
        }
    }
    
    public void equip() {
        intelligence.transporter = create_transporter();
        //weaponry = get_weaponry();
    }

    protected abstract ITransporter create_transporter(); 
    //protected abstract IWeaponry get_weaponry();

   void FixedUpdate() {
       intelligence.update();
   }

   public void OnMouseDown() {
       
       Vector2 mousePos = ui.input.Input.instance.mouse_world_position;
       Ray2D ray = new Ray2D(
           mousePos,
           new Vector2(0.5f,1f)
       );
       
       divisible_body.split_by_ray(ray);
   }
    
    

    
    
}


}