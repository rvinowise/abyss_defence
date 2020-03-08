using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rvinowise;
using rvinowise.rvi.contracts;
using rvinowise.units.parts.limbs.legs;
using rvinowise.units.parts.transport;


namespace rvinowise.units.parts.humanoid {

public class Legs: Equipment_controller,
    ITransporter {
    
    
    private Rigidbody2D rigidbody;
    
    /* Equipment_controller interface */
    public override IEnumerable<Child> tools { get; }

    public override IEquipment_controller copy_empty_into(User_of_equipment dst_host) {
        throw new NotImplementedException();
    }

    public override void add_tool(Child child) {
        throw new NotImplementedException();
    }

    public override void update() {
        execute_commands();
    }

    protected override void execute_commands() {
        move_in_direction(command_batch.moving_direction_vector);
        rotate_to_direction(command_batch.face_direction_degrees);
    }

    protected override void init_components() {
        rigidbody = game_object.GetComponent<Rigidbody2D>();
        Contract.Requires(rigidbody != null);
    }
    
    /* ITransporter interface */

    public transport.Command_batch command_batch { get; } = new transport.Command_batch();
    

    public float get_possible_rotation() {
        return 110f;
    }

    public float get_possible_impulse() {
        return 1f;
    }


    /* Legs itself */

    private float acceleration = 0.339f * Settings.scale;

    public Legs() : base() { }

    public Legs(User_of_equipment in_user):base(in_user) {
        
    }

    public void move_in_direction(Vector2 direction) {
        Vector2 force = direction * get_possible_impulse() * Time.deltaTime;
        rigidbody.MovePosition(rigidbody.position + force);
    }
    
    public void rotate_to_direction(float face_direction) {
        transform.rotate_to(
            face_direction, 
            get_possible_rotation() * rvi.Time.deltaTime
        );
    }
}
}