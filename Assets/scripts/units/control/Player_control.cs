using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static rvi.Input;
//using static UnityEngine.Input;
using geometry2d;
using static geometry2d.Directions;

namespace units {

public class Player_control: IControl  {
    public float horizontal {get;private set;}
    public float vertical {get;private set;}
    public float rotation {get;private set;}
    
    private Transform transform;

    public Player_control(Transform in_transform) {
        transform = in_transform;
    }

    public void read_input() {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
        
        Vector2 mousePos = rvi.Input.mouse_world_position();
        float desired_direction = transform.degrees_to(mousePos);
        rotation = desired_direction;
    }
}

}