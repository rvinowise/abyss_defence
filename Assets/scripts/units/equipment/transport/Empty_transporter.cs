using System;
using System.Collections;
using System.Collections.Generic;
using rvinowise.unity.units.parts.transport;
using UnityEngine;
using rvinowise.unity.extensions;


namespace rvinowise.unity.units.parts {
    
/* provides information about possible speed and rotation for a moving Unit */
public class Empty_transporter: ITransporter {
    private ITransporter _transporter_implementation;

    public float possible_rotation { get; set; } = 0f;
    public float possible_impulse { get; set; } = 0f;

    public Quaternion direction_quaternion { get; }

    public transport.Command_batch command_batch { get; }


    public void rotate_to_direction(float direction) {
        throw new NotImplementedException();
    }

    public void move_in_direction(float direction) {
        throw new NotImplementedException();
    }

    public void move_in_direction(Vector2 moving_direction) {
        throw new NotImplementedException();
    }

    public ITransporter get_copy() {
        return new Empty_transporter();
    }

    public IEnumerable<IChild_of_group> tools { get; }
    public void add_tool(IChild_of_group compound_object) {
        throw new NotImplementedException();
    }

    public void add_to_user(IChildren_groups_host in_user) {
        throw new NotImplementedException();
    }

    public IChildren_group copy_empty_into(IChildren_groups_host dst_host) {
        throw new NotImplementedException();
    }

    public void init() {
        throw new NotImplementedException();
    }

    

    public void update() {
        throw new NotImplementedException();
    }
}

}