using System.Collections;
using System.Collections.Generic;
using System;
using rvinowise.unity.geometry2d;
using rvinowise.unity.units.parts;
using rvinowise.unity.units.control;
using rvinowise.unity.units.control.human;
using rvinowise.unity.units.control.spider;
using rvinowise.unity.effects.liquids;
using rvinowise.unity.units.gore;
using rvinowise.unity.units.parts.limbs;
using rvinowise.unity.units.parts.limbs.creeping_legs;
using rvinowise.unity.units.parts.transport;
using UnityEngine;
using rvinowise.unity.extensions;
using rvinowise.unity;


namespace rvinowise.unity.units {

[RequireComponent(typeof(PolygonCollider2D))]
public abstract class Creature: Turning_element
{
    /* IChildren_groups_host interface */
    public virtual ITransporter transporter { get; protected set; }
    public virtual IWeaponry weaponry { get; set; }
    
    
    /* Creature itself */
    [HideInInspector]
    public Divisible_body divisible_body;
    [HideInInspector]
    public Bleeding_body bleeding_body;

    
    Intelligence intelligence;

    
    protected override void Awake()
    {
        base.Awake();
        init_components();
    }

    protected virtual void Start() {
        create_equipment();
        
    }

    private void init_components() {
        divisible_body = gameObject.GetComponent<Divisible_body>();
    }

    protected virtual void create_equipment() {}


    protected virtual void fill_equipment_with_children() {}


   void FixedUpdate() {
       //intelligence.update();
       
       //transporter?.update();
       //weaponry?.update();
   }
   
   void Update() {
       
   }

  

}


}