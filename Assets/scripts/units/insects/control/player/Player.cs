using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rvinowise;
using geometry2d;

using Input = rvinowise.ui.input.Input;


namespace rvinowise.units.control.spider {

public class Player: Spider {

    public Player(Transform in_transform) {
        body_transform = in_transform;
    }
    protected override void read_input() {
        read_transporter_input();
    }
    
    private void read_transporter_input() {
        if (transporter == null) {
            return;
        }
        transporter.command_batch.moving_direction_vector = read_moving_direction();
        transporter.command_batch.face_direction_quaternion = read_face_direction();

        Vector2 read_moving_direction() {
            Vector2 direction_vector = Input.instance.moving_vector;
            return direction_vector.normalized;
        }

        Quaternion read_face_direction() {
            Vector2 mousePos = Input.instance.mouse_world_position;
            Quaternion needed_direction = (mousePos - (Vector2) body_transform.position).to_quaternion();

            save_last_rotation(needed_direction);

            return needed_direction;
        }
    }
}
}