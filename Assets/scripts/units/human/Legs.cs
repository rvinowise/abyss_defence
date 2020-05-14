using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rvinowise;
using rvinowise.rvi.contracts;
using rvinowise.units.parts.limbs.legs;
using rvinowise.units.parts.transport;


namespace rvinowise.units.parts.humanoid {

public class Legs: 
    ITransporter 
{
    
    /* ITransporter interface */

    public transport.Command_batch command_batch { get; } = new transport.Command_batch();
    Command_batch IExecute_commands.command_batch => command_batch;
    
    
    public void update() {
        execute_commands();
    }

    protected void execute_commands() {
        move_in_direction(command_batch.moving_direction_vector);
        rotate_to_direction(command_batch.face_direction_degrees);
    }

    public float get_possible_rotation() {
        return 110f;
        //return 0f;
    }

    public float get_possible_impulse() {
      return 1f;
    }

    public Quaternion direction_quaternion {
        get { return user.transform.rotation; }
    }


    /* legs itself */
    
    private GameObject user;
    
    private Rigidbody2D moved_body;

    private float acceleration = 0.339f * rvinowise.Settings.scale;

    public Legs(GameObject in_user) {
        user = in_user;
        init_components();
    }
    
    public Legs() {
        init_components();
    }
    
    protected void init_components() {
        moved_body = user.GetComponent<Rigidbody2D>();
        Contract.Requires(moved_body != null);
    }


    public void move_in_direction(Vector2 direction) {
        Vector2 force = direction * get_possible_impulse() * Time.deltaTime;
        moved_body.MovePosition(moved_body.position + force);
    }
    
    public void rotate_to_direction(float face_direction) {
        user.transform.rotate_to(
            face_direction, 
            get_possible_rotation() * rvi.Time.deltaTime
        );
    }
}
}