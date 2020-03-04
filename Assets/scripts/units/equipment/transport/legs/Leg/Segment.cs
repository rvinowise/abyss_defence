using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rvinowise;


namespace rvinowise.units.equipment.limbs.legs {

public class Segment: limbs.Segment {

    /* the most comfortable direction if the body isn't moving.
     determined during construction*/
    public Quaternion desired_relative_direction_standing { get; set; } //relative to attachment
    
    public Segment(string name):base(name) {
    }

}
}