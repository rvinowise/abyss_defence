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

    
    protected virtual void Awake()
    {
        base.Awake();
        init_components();
    }

    protected virtual void Start() {
        //base.Start();
        create_equipment();
        init_intelligence();
        if (divisible_body.needs_initialisation) {
            fill_equipment_with_children();
            divisible_body.needs_initialisation = false;
        }
        else {
            
        }
    }

    private void init_components() {
        divisible_body = gameObject.GetComponent<Divisible_body>();
    }

    protected virtual void create_equipment() {}

    private void init_intelligence() {
        intelligence = new control.spider.Player(transform);
        intelligence.transporter = transporter;
    }

    protected virtual void fill_equipment_with_children() {}


   void FixedUpdate() {
       intelligence.update();
       
       transporter?.update();
       weaponry?.update();
   }
   
   void Update() {
       
   }

   public void OnMouseDown() {
       
      /*  Vector2 mousePos = rvinowise.unity.ui.input.Input.instance.mouse_world_position;
       Polygon wedge = Polygon_splitter.get_wedge_from_ray(
           new Ray2D(
               mousePos,
               new Vector2(0.5f, 1f)
           )
       );
       
       divisible_body.remove_polygon(wedge); */
   }

   /*public void OnCollisionEnter2D(Collision2D other) {
       Projectile collided_projectile = other.gameObject.GetComponent<Projectile>();
       if (collided_projectile != null) {

           Vector2 contact_point = other.GetContact(0).point;
           
           Polygon removed_polygon = collided_projectile.get_damaged_area(
               contact_point
           );
           Debug_drawer.instance.draw_polygon_debug(removed_polygon);
           
           var pieces = divisible_body.remove_polygon(removed_polygon);
           //push_pieces_away(pieces, contact_point, collided_projectile.last_physics.velocity.magnitude);
           
           //Destroy(collided_projectile.gameObject);
       }
   }

   private void push_pieces_away(
       IEnumerable<GameObject> pieces,
       Vector2 contact_point,
       float force
   ) {
       foreach (var piece in pieces) {
           Vector2 push_vector = 
               ((Vector2)piece.transform.position - contact_point).normalized *
               force;
           var rigidbody = piece.GetComponent<Rigidbody2D>();
           rigidbody.AddForce(push_vector, ForceMode2D.Impulse);
           //rigidbody.AddTorque(50f);
       }
   }*/
   
   public void OnDrawGizmos() {
       ((Creeping_leg_group)transporter)?.on_draw_gizmos();
   }
}


}