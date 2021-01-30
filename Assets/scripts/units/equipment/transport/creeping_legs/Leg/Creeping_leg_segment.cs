using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rvinowise.unity.extensions;

using rvinowise;


namespace rvinowise.unity.units.parts.limbs.creeping_legs {

public class Creeping_leg_segment: limbs.Segment {

    /* the most comfortable direction_quaternion if the body isn't moving.
     determined during construction*/
    public Quaternion desired_relative_direction_standing { get; set; } //relative to local_position
    
    public void mirror_from(Creeping_leg_segment src) {
        base.mirror_from(src);

        desired_relative_direction_standing =
            Quaternion.Inverse(src.desired_relative_direction_standing);
        
    }
}
}