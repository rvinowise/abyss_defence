using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace rvinowise.units {

public interface IControl {
    float vertical {get;}
    float horizontal {get;}
    
    float moving_direction_degrees { get; } // i want to move in this direction (in degrees)
    Vector2 moving_direction_vector { get; } // i want to move in this direction (in vector)
    float face_direction_degrees {get;} // i want to face this direction

    void read_input();
}

}