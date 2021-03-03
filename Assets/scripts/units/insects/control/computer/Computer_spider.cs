using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rvinowise.unity.extensions;
using rvinowise.unity.units.control;

//using static UnityEngine.Input;
using rvinowise.unity.geometry2d;
using rvinowise.unity.units.parts;

using Input = rvinowise.unity.ui.input.Player_input;

namespace rvinowise.unity.units.control.spider {

public class Computer_spider: Strategic_intelligence  {
    
    
    private Transform target;

    protected override void read_input() {
        transporter.command_batch.moving_direction_vector = read_moving_direction();
        transporter.command_batch.face_direction_quaternion = read_face_direction();
    }

    private Vector2 read_moving_direction() {
        if (unit_commands.attack_target != null) {
            return (unit_commands.attack_target.position - transform.position).normalized;
        }
        return Vector2.zero;
    }
    
    private Quaternion read_face_direction() {
        if (unit_commands.attack_target != null) {
            return transform.quaternion_to(unit_commands.attack_target.position);
        }
       return Quaternion.identity;
    }

}

}