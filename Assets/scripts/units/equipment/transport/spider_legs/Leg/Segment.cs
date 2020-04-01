using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rvinowise;


namespace rvinowise.units.parts.limbs.legs {

public class Segment: limbs.Segment {

    /* the most comfortable direction_quaternion if the body isn't moving.
     determined during construction*/
    public Quaternion desired_relative_direction_standing { get; set; } //relative to local_position
    
    
    /* limbs.Segment interface */
    public Segment(string name):base(name) {
    }

}
}