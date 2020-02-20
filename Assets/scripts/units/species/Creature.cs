using System.Collections;
using System.Collections.Generic;
using System;
using rvinowise.units.equipment;
using UnityEngine;


namespace rvinowise.units {

[RequireComponent(typeof(PolygonCollider2D))]
public abstract class Creature : MonoBehaviour {
    
    
    protected Divisible_body divisible_body;
    protected User_of_equipment userOfEquipment;
    
    /* equipment used by this gameObject */
    public ITransporter transporter;
    public IWeaponry weaponry;
    
    IControl control;

    
    protected virtual void Awake()
    {
        divisible_body = gameObject.GetComponent<Divisible_body>();
        userOfEquipment = GetComponent<User_of_equipment>();
        control = new Player_control(this.transform);

        equip();
    }

    public void copy_from(Creature src_creature) {
        //user_of_tools
        transporter = src_creature.transporter.get_copy();
        //weaponry = src_creature.weaponry.get_copy();
    }
    
    public void equip() {
        transporter = create_transporter();
        //weaponry = get_weaponry();
    }

    protected abstract ITransporter create_transporter(); 
    protected abstract IWeaponry get_weaponry();

   void FixedUpdate() {
        control.read_input();
        apply_control(control);
   }
    
    private void apply_control(IControl control) {
        
        transform.rotate_to(
            control.rotation, 
            transporter.get_possible_rotation() * rvi.Time.deltaTime
        );
        
        transform.position += (Vector3)new Vector2(
            Math.Sign(control.horizontal) * 
                transporter.get_possible_impulse() * rvi.Time.deltaTime, 
            
            Math.Sign(control.vertical) * 
                transporter.get_possible_impulse() * rvi.Time.deltaTime
        );
        /*Debug.DrawRay(
            transform.position, 
            Directions.degrees_to_vector(control.rotation)
            ); */
    }

    void OnMouseDown() {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Ray2D ray = new Ray2D(
                mousePos,
                new Vector2(0.5f,1f)
            );
        divisible_body.split_by_ray(ray);
        /*divisible_body.split_by_ray(
            new Ray2D(
                Camera.main.ScreenToWorldPoint(
                    new Vector2(Input.mousePosition.x, 
                                Input.mousePosition.y)
                ),
                new Vector2(0.5f,1f)
            )
        );*/
    }

    
    public enum Species {
        Normal_spider,
        Hexapod_spider
    }
    
}


}