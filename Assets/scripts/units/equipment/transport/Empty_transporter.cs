﻿using System;
using System.Collections;
using System.Collections.Generic;
using units.equipment.transport;
using UnityEngine;

namespace rvinowise.units.equipment {
    
/* provides information about possible speed and rotation for a moving Unit */
public class Empty_transporter: ITransporter {
    private ITransporter _transporter_implementation;

    public float get_possible_rotation() {
        return 0;
    }

    public float get_possible_impulse() {
        return 0;
    }

    public Command_batch command_batch { get; }

    public void command(Command_batch command_batch) {
        
    }

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

    public IEnumerable<Tool> tools { get; }
    public void add_tool(Tool tool) {
        throw new NotImplementedException();
    }

    public void add_to_user(User_of_equipment in_user) {
        throw new NotImplementedException();
    }

    public IEquipment_controller copy_empty_into(User_of_equipment dst_host) {
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