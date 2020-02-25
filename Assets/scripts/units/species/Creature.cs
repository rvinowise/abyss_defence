using System.Collections;
using System.Collections.Generic;
using System;
using rvinowise.units.equipment;
using UnityEngine;


namespace rvinowise.units {

[RequireComponent(typeof(PolygonCollider2D))]
public abstract class Creature : MonoBehaviour {
    
    public bool needs_initialisation = true; //it was added ineditor and created from scratch
    
    protected Divisible_body divisible_body;
    protected User_of_equipment user_of_equipment;
    
    /* equipment used by this gameObject */
    public ITransporter transporter {
        get { return user_of_equipment.transporter; }
        set => user_of_equipment.transporter = value;
    }
    public IWeaponry weaponry {
        get { return user_of_equipment.weaponry; }
        set => user_of_equipment.weaponry = value;
    }
    
    IControl control;

    
    protected virtual void Awake()
    {
        divisible_body = gameObject.GetComponent<Divisible_body>();
        control = new Player_control(this.transform);
        user_of_equipment = GetComponent<User_of_equipment>();
        user_of_equipment.control = control;

        if (needs_initialisation) {
            equip();
            needs_initialisation = false;
        }
    }
    
    public void equip() {
        transporter = create_transporter();
        //weaponry = get_weaponry();
    }

    protected abstract ITransporter create_transporter(); 
    //protected abstract IWeaponry get_weaponry();

   void FixedUpdate() {
        control.read_input();
        apply_control(control);
   }
    
    private void apply_control(IControl control) {

        transporter.rotate_to_direction(control.face_direction_degrees);
        transporter.move_in_direction(control.moving_direction_vector);
        
        
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
    
}


}