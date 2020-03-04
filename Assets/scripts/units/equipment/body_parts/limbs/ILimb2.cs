using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rvinowise;


namespace rvinowise.units.equipment.limbs {

public interface ILimb2 {

    Segment segment1 { get; }
    Segment segment2 { get; }

    int folding_direction { get; set; } //1 of -1

    /* Child interface */
     Transform host { get; }

     Vector2 attachment { get; set; }

    

}
}