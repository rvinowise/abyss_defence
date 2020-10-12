using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rvinowise;
using rvinowise.rvi.contracts;
using rvinowise.units.parts.limbs.creeping_legs;
using rvinowise.units.parts.transport;
using rvinowise.units.parts.limbs;
using geometry2d;

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

    public float possible_rotation {get;set;} = 400f;
    public float possible_impulse { get; set; } =1f;

    public Quaternion direction_quaternion {
        get { return turning_element.rotation; }
    }


    /* legs itself */
    
    private GameObject user;
    
    private Rigidbody2D rigidbody;
    private Turning_element turning_element;

    private float acceleration = 0.339f * rvinowise.Settings.scale;

    public Legs(GameObject in_user) {
        user = in_user;
        init_components();
    }
    
    public Legs() {
        init_components();
    }
    
    protected void init_components() {
        rigidbody = user.GetComponent<Rigidbody2D>();
        turning_element = user.GetComponent<Turning_element>();
        turning_element.rotation_acceleration = possible_rotation;
        
        Contract.Requires(rigidbody != null);
        Contract.Requires(turning_element != null);
    }


    public void move_in_direction(Vector2 direction) {
        Vector2 force = direction * possible_impulse * Time.deltaTime;
        rigidbody.MovePosition(rigidbody.position + force);
    }
    
    public void rotate_to_direction(float face_direction) {
        turning_element.target_quaternion = Directions.degrees_to_quaternion(face_direction);
        turning_element.rotate_to_desired_direction();
    }
}
}