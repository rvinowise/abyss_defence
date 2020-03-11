using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//using static UnityEngine.Input;
using geometry2d;
using rvinowise.units.parts;
using static geometry2d.Directions;

namespace rvinowise.units.control {

public class Computer: Intelligence  {
    public float horizontal {get;private set;}
    public float vertical {get;private set;}
    
    public float moving_direction_degrees {
        get { return _moving_direction_degrees;}
        private set { _moving_direction_degrees = value; }
    }
    private float _moving_direction_degrees;

    public Vector2 moving_direction_vector {
        get { return _moving_direction_vector; }
        private set { _moving_direction_vector = value; }
    }
    private Vector2 _moving_direction_vector;

    public float face_direction_degrees {
        get { return _face_direction_degrees;}
        private set { _face_direction_degrees = value; }
    }
    private float _face_direction_degrees;

    private Transform transform;


    public void read_input() {
        read_moving_direction(
             out _moving_direction_degrees,
             out _moving_direction_vector
        );
        read_face_direction(
            out _face_direction_degrees
        );

    }

    public void read_moving_direction(out float degrees, out Vector2 vector) {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
        
        Vector2 direction_vector = new Vector2(horizontal,vertical);
        vector = direction_vector.normalized;
        degrees = direction_vector.to_dergees();
    }
    
    public void read_face_direction(out float degrees) {
        Vector2 mousePos = rvi.Input.mouse_world_position();
        degrees = transform.degrees_to(mousePos);
    }

    public Computer(User_of_equipment in_user) : base(in_user) { }
}

}