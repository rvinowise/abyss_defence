using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using static UnityEngine.Input;
using geometry2d;
using rvinowise.units.control;
using rvinowise.units.equipment;
using units.control;
using static geometry2d.Directions;

namespace rvinowise.units.control {

public class Player_control: Intelligence  {
    
    private Transform transform;

    public Player_control(
        Transform in_transform,
        User_of_equipment in_user)
    :base(in_user) 
    {
        transform = in_transform;
    }

    public override void update() {
        read_input();
        apply_to_equipment();
    }
    
    private void apply_to_equipment() {
        
    }

    public void read_input() {
        read_moving_direction(
             out transporter.command_batch.moving_direction_degrees,
             out transporter.command_batch.moving_direction_vector
        );
        read_face_direction(
            out transporter.command_batch.face_direction_degrees
        );

    }

    public void read_moving_direction(out float degrees, out Vector2 vector) {
        /*horizontal = rvi.Math.sign_or_zero(Input.GetAxis("Horizontal"));
        vertical = rvi.Math.sign_or_zero(Input.GetAxis("Vertical"));*/
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        
        Vector2 direction_vector = new Vector2(horizontal,vertical);
        vector = direction_vector.normalized;
        degrees = direction_vector.to_dergees();
    }
    
    public void read_face_direction(out float degrees) {
        Vector2 mousePos = rvi.Input.mouse_world_position();
        degrees = transform.degrees_to(mousePos);
    }
}

}