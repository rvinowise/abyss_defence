using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rvinowise;


namespace rvinowise.units.parts.limbs.arms {

public class Segment: limbs.Segment {

    public Quaternion desired_idle_direction; //aim there if no task is given
    
    public Segment(string name):base(name) {
        
    }

    public override void mirror_from(limbs.Segment src) {
        base.mirror_from(src);
        if (src is arms.Segment arms_src) {
            this.desired_idle_direction = Quaternion.Inverse(arms_src.desired_idle_direction);
        }
    }
    

}
}