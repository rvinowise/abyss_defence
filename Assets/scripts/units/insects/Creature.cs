using System.Collections;
using System.Collections.Generic;
using System;
using geometry2d;
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
    public Divisible_body divisible_body;

    
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
       
       //divisible_body.split_by_ray(ray);
   }

   public void OnCollisionEnter2D(Collision2D other) {
       Bullet other_bullet = other.gameObject.GetComponent<Bullet>();
       if (other_bullet != null) {
           //Rigidbody2D other_rigid = other.rigidbody;

           Ray2D ray_of_impact = new Ray2D(
               other_bullet.last_physics.position,
               other_bullet.last_physics.velocity
           );
           
           
           Polygon wedge = Polygon_splitter.get_wedge_from_ray(ray_of_impact);
           Debug_drawer.instance.draw_polygon_debug(wedge);
           
           var pieces = divisible_body.split_by_ray(ray_of_impact);
           push_pieces_away(pieces, ray_of_impact, other_bullet.last_physics.velocity.magnitude);
           
           Destroy(other_bullet.gameObject);
       }
   }

   private void push_pieces_away(IEnumerable<GameObject> pieces, Ray2D ray, float force) {
       foreach (var piece in pieces) {
           Vector2 push_vector = 
               ((Vector2)piece.transform.position - ray.origin).normalized *
               force;
           var rigidbody = piece.GetComponent<Rigidbody2D>();
           rigidbody.AddForce(push_vector / 100f);
           rigidbody.AddTorque(50f);
       }
   }
}


}