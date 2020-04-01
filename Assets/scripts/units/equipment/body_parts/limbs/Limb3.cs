using System;
using System.Collections;
using System.Collections.Generic;
using geometry2d;
using UnityEngine;
using rvinowise;


namespace rvinowise.units.parts.limbs {

public class Limb3: Limb2 {

    public virtual Segment segment3 { get; set; } //an appendage at the end, to direct the children

    public Limb3() {
        
    }
    public Limb3(Segment in_segment1, Segment in_segment2, Segment in_segment3) {
        segment1 = in_segment1;
        segment2 = in_segment2;
        segment3 = in_segment3;

    }
    
    public virtual void rotate_to_desired_directions() {
        base.rotate_to_desired_directions();
        segment3.rotate_to_desired_direction();
    }

    public void rotate_to_orientation(Orientation needed_orientation) {
        set_desired_directions_by_position(needed_orientation.position);
        segment3.desired_direction = needed_orientation.rotation;
        rotate_to_desired_directions();
    }
    
    public override void preserve_possible_rotations() {
        base.preserve_possible_rotations();
        segment3.preserve_possible_rotation();
    }
    
    public bool at_desired_rotation() {
        return (
            base.at_desired_rotation() &&
            this.segment3.at_desired_rotation()
        );

    }
}
}