using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using geometry2d;
using UnityEngine;
using rvinowise;
using rvinowise.rvi.contracts;
using static geometry2d.Directions;


namespace rvinowise.units.parts.limbs {

public partial class Limb2: Child {
    public Segment segment1 { get; set; } //beginning (root)
    public Segment segment2 { get; set; } //ending (leaf)

    public int folding_direction { get; set; } //1 of -1
    
    protected virtual Limb2.Debug debug_limb { get; set; }


    public void reach_point(Vector2 desired_position) {
        
    }
    
    public void set_desired_directions_by_position(Vector2 desired_position) {
        Contract.Requires(folding_direction != 0, "folding_direction is needed for bending the arm");
        
        //all positions are in global coordinates
        float distance_to_aim = 
            ((Vector2)segment1.position).distance_to(desired_position);
        float segment1_angle_offset = 
            geometry2d.Triangles.get_angle_by_lengths(
                segment1.length,
                distance_to_aim,
                segment2.length
            );
        if (float.IsNaN(segment1_angle_offset)) {
            // leg can't reach the desired position physically
            debug_limb.draw_lines(Color.magenta,0.5f);
            //return;
            segment1_angle_offset = 0f; // assuming the Leg can become straight
        }
        
        segment1.desired_direction = degrees_to_quaternion(
            segment1.position.degrees_to(desired_position) +
            (folding_direction *  segment1_angle_offset )
        );

        Vector2 segment2_position =
            segment1.position +
            (Vector2) (segment1.desired_direction * segment1.tip);

        segment2.desired_direction = degrees_to_quaternion(
            segment2_position.degrees_to(desired_position)
        );
    }

    public bool at_desired_rotation() {
        return (
            this.segment1.at_desired_rotation() &&
            this.segment2.at_desired_rotation()
        );

    }
    
    
    

    
}
}