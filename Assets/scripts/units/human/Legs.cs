using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rvinowise;
using rvinowise.rvi.contracts;
using units.equipment.transport;


namespace rvinowise.units.equipment.humanoid {

public class Legs: Equipment_controller,
    ITransporter {
    
    
    private Rigidbody2D rigidbody;
    
    /* Equipment_controller interface */
    public override IEnumerable<Tool> tools { get; }
    protected override Command_batch new_command_batch() {
        throw new NotImplementedException();
    }

    public override IEquipment_controller copy_empty_into(User_of_equipment dst_host) {
        throw new NotImplementedException();
    }

    public override void add_tool(Tool tool) {
        throw new NotImplementedException();
    }

    public override void update() {
        base.update();
    }

    protected override void execute_commands() {
        
    }

    protected override void init_components() {
        rigidbody = game_object.GetComponent<Rigidbody2D>();
        Contract.Requires(rigidbody != null);
    }
    
    /* ITransporter interface */

    public float get_possible_rotation() {
        return 360f;
    }

    public float get_possible_impulse() {
        return 1f;
    }

 

    public void command(Command_batch command_batch) {
    }
    
    
    /* Legs itself */

    private float acceleration = 0.1f;
    
    Legs() {
        
    }

    public void move_in_direction(Vector2 direction) {
        Vector2 force = direction * get_possible_impulse() * Time.deltaTime;
        rigidbody.AddForce(direction);
    }
    
    public void rotate_to_direction(float face_direction) {
        transform.rotate_to(
            face_direction, 
            get_possible_rotation() * rvi.Time.deltaTime
        );
    }
}
}