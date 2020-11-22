using System;
using System.Collections;
using System.Collections.Generic;
using rvinowise.unity.geometry2d;
using UnityEngine;
using rvinowise.unity.extensions;

using rvinowise;


namespace rvinowise.unity.units.parts.limbs {

public class Limb3: Limb2 {

    public virtual Segment segment3 { get; set; } //an appendage at the end, to direct the children

    public Limb3() {
        
    }
    
    
    public virtual void rotate_to_desired_directions() {
        base.rotate_to_desired_directions();
        segment3.rotate_to_desired_direction();
    }
    
    public virtual void jump_to_desired_directions() {
        base.jump_to_desired_directions();
        segment3.jump_to_desired_direction();
    }

    public virtual void rotate_to_orientation(Orientation needed_orientation) {
        set_desired_directions_by_position(needed_orientation.position);
        segment3.target_quaternion = needed_orientation.rotation;
        rotate_to_desired_directions();
    }
    
    public override void preserve_possible_rotations() {
        base.preserve_possible_rotations();
        segment3.collide_with_rotation_borders();
    }
    
    public bool at_desired_rotation() {
        return (
            base.at_desired_rotation() &&
            this.segment3.at_desired_rotation()
        );

    }
}
}