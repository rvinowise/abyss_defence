using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace rvi {

using unity = UnityEngine;

public class Input: unity::Input
{
    public static Vector2 mouse_world_position()
    {
        return Camera.main.ScreenToWorldPoint(unity::Input.mousePosition);
    }
}

}