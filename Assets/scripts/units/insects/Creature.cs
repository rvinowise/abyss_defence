using System.Collections;
using System.Collections.Generic;
using System;
using rvinowise.units.parts;
using rvinowise.units.control;
using rvinowise.units.control.human;
using rvinowise.units.control.spider;
using rvinowise.units.parts.transport;
using UnityEngine;

namespace rvinowise.units {

[RequireComponent(typeof(PolygonCollider2D))]
public abstract class Creature: MonoBehaviour
{
    /* IChildren_groups_host interface */
    public virtual ITransporter transporter { get; protected set; }
    public virtual IWeaponry weaponry { get; set; }
    
    
    /* Creature itself */
    protected Divisible_body divisible_body;

    
    Intelligence intelligence;

    
    protected virtual void Awake()
    {
        init_components();

        create_equipment();
        init_intelligence();
        if (divisible_body.needs_initialisation) {
            fill_equipment_with_children();
            divisible_body.needs_initialisation = false;
        }
    }

    private void init_components() {
        divisible_body = gameObject.GetComponent<Divisible_body>();
    }

    protected virtual void create_equipment() {}

    private void init_intelligence() {
        intelligence = new Player(transform);
        intelligence.transporter = transporter;
    }

    protected virtual void fill_equipment_with_children() {}


   void FixedUpdate() {
       intelligence.update();
   }
   
   void Update() {
       transporter?.update();
       weaponry?.update();
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