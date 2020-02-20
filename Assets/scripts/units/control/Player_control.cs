using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static rvi.Input;
//using static UnityEngine.Input;
using geometry2d;
using static geometry2d.Directions;

namespace rvinowise.units {

public class Player_control: IControl  {
    public float horizontal {get;private set;}
    public float vertical {get;private set;}
    public float rotation {get;private set;}
    
    private Transform transform;

    public Player_control(Transform in_transform) {
        transform = in_transform;
    }

    public void read_input() {
        horizontal = rvi.Math.sign_or_zero(Input.GetAxis("Horizontal"));
        vertical = rvi.Math.sign_or_zero(Input.GetAxis("Vertical"));
        //Debug.Log("horizontal:" + horizontal);
        //Debug.Log("vertical:" + vertical);
        
        Vector2 mousePos = rvi.Input.mouse_world_position();
        float desired_direction = transform.degrees_to(mousePos);
        rotation = desired_direction;
    }
}

}