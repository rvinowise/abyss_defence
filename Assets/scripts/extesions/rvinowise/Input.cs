using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace rvinowise.rvi {

using unity = UnityEngine;

public class Input: unity::Input
{
    public static Vector2 mouse_world_position()
    {
        return Camera.main.ScreenToWorldPoint(unity::Input.mousePosition);
    }

    public static int mouse_wheel_steps() {
        float wheel_movement = Input.GetAxis("Mouse ScrollWheel");
        return (int)System.Math.Round(wheel_movement * 10);
    }
}

}