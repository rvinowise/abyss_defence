using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using geometry2d;
using units.limbs;

namespace units {

[RequireComponent(typeof(PolygonCollider2D))]
[RequireComponent(typeof(User_of_tools))]
public class Creature : MonoBehaviour
{
    Divisible_body divisible_body;
    Leg_controller leg_controller;
    IControl control;
    
    /* interface of IUser_of_tools */
    public IEnumerable<ITool_controller> tool_controllers {
        get {
            return _tool_controllers;
        }
        private set {
            _tool_controllers = value;
        }
    }
    private IEnumerable<ITool_controller> _tool_controllers;

    private void Awake()
    {
        divisible_body = gameObject.GetComponent<Divisible_body>();
        leg_controller = gameObject.GetComponent<Leg_controller>();
        
        control = new Player_control(this.transform);

    }



   void FixedUpdate() {
        control.read_input();
        //apply_control(control);
   }
    
    private void apply_control(IControl control) {
        
        transform.rotate_to(
            control.rotation, 
            leg_controller.get_possible_impulse() * rvi.Time.deltaTime
        );
        
        transform.position += (Vector3)new Vector2(
            Math.Sign(control.horizontal) * 
                leg_controller.get_possible_impulse() * rvi.Time.deltaTime, 
            
            Math.Sign(control.vertical) * 
                leg_controller.get_possible_impulse() * rvi.Time.deltaTime
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
        Divisible_body divisible_body = GetComponent<Divisible_body>(); 
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