using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rvinowise;
using rvinowise.rvi.contracts;
using rvinowise.units.parts.limbs.legs;
using rvinowise.units.parts.transport;


namespace rvinowise.units.parts.humanoid {

public class Legs: Children_group,
    ITransporter {
    
    
    private Rigidbody2D moved_body;
    
    /* Children_group interface */
    public override IEnumerable<Child> children { get; }


    public override IChildren_group copy_empty_into(IChildren_groups_host dst_host) {
        throw new NotImplementedException();
    }

    public override void add_child(Child child) {
        throw new NotImplementedException();
    }

    

    public void update() {
        execute_commands();
    }

    protected void execute_commands() {
        move_in_direction(command_batch.moving_direction_vector);
        rotate_to_direction(command_batch.face_direction_degrees);
    }

    protected override void init_components() {
        moved_body = game_object.GetComponent<Rigidbody2D>();
        Contract.Requires(moved_body != null);
    }
    
    /* ITransporter interface */

    public transport.Command_batch command_batch { get; } = new transport.Command_batch();
    Command_batch IExecute_commands.command_batch => command_batch;
    

    public float get_possible_rotation() {
        return 110f;
    }

    public float get_possible_impulse() {
      return 1f;
    }

    public Quaternion direction_quaternion {
        get { return transform.rotation; }
    }


    /* Legs itself */

    private float acceleration = 0.339f * rvinowise.Settings.scale;

    public Legs() : base() { }

    public Legs(IChildren_groups_host in_user):base(in_user) {
        
    }

    public void move_in_direction(Vector2 direction) {
        Vector2 force = direction * get_possible_impulse() * Time.deltaTime;
        moved_body.MovePosition(moved_body.position + force);
    }
    
    public void rotate_to_direction(float face_direction) {
        transform.rotate_to(
            face_direction, 
            get_possible_rotation() * rvi.Time.deltaTime
        );
    }
}
}