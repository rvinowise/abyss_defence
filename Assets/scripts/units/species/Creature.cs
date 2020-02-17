using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using geometry2d;
using units.limbs;

namespace units {

[RequireComponent(typeof(PolygonCollider2D))]
public class Creature : MonoBehaviour {
    
    public Species species;
    
    protected Divisible_body divisible_body;
    protected User_of_tools user_of_tools;
    
    protected ITransporter transporter;
    
    IControl control;
    

    protected virtual void Awake()
    {
        divisible_body = gameObject.GetComponent<Divisible_body>();
        user_of_tools = GetComponent<User_of_tools>();
        transporter = gameObject.GetComponent<Leg_controller>();
        
        control = new Player_control(this.transform);

    }



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