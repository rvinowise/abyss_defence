using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using geometry2d;
using UnityEngine;
using rvinowise;
using rvinowise.rvi.contracts;
using UnityEngine.Assertions;
using static geometry2d.Directions;


namespace rvinowise.units.parts.limbs {

public partial class Limb2: ICompound_object {
    public Segment segment1 { get; set; } //beginning (root)
    public Segment segment2 { get; set; } //ending (leaf)

    public geometry2d.Side folding_direction { get; set; } //1 of -1
    
    public Transform parent {
        get { return segment1.parent;}
        set { segment1.parent = value; }
    }
    public Vector2 local_position {
        get { return segment1.local_position; }
        set { segment1.local_position = value; }
    }
    
    public GameObject main_object {
        get { return segment1.gameObject; }
    }
    
    protected virtual Limb2.Debug debug_limb { get; set; }

    
    
    public virtual void rotate_to_desired_directions() {
        segment1.rotate_to_desired_direction();
        segment2.rotate_to_desired_direction();
    }
    public virtual void jump_to_desired_directions() {
        segment1.jump_to_desired_direction();
        segment2.jump_to_desired_direction();
    }
    
    public virtual void preserve_possible_rotations() {
        segment1.collide_with_rotation_borders();
        segment2.collide_with_rotation_borders();
    }
    
    public void set_desired_directions_by_position(Vector2 desired_position) {
        Contract.Requires((folding_direction != Side.NONE) && (folding_direction != null),
            "folding_direction is needed for bending the arm"
        );
        
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
            // limb can't reach the desired position physically
            debug_limb.draw_lines(Color.magenta,0.5f);
            //return;
            segment1_angle_offset = 0f; // assuming the Limb can become straight
        }
        
        segment1.target_quaternion = degrees_to_quaternion(
            segment1.position.degrees_to(desired_position) +
            (folding_direction *  segment1_angle_offset )
        );

        Vector2 segment2_position =
            segment1.position +
            (Vector2) (segment1.target_quaternion * segment1.tip);

        segment2.target_quaternion = degrees_to_quaternion(
            segment2_position.degrees_to(desired_position)
        );
    }

    public bool at_desired_rotation() {
        return (
            this.segment1.at_desired_rotation() &&
            this.segment2.at_desired_rotation()
        );

    }


    public bool hold_onto_point(Vector2 holding_point) {
        
        float distance_to_aim = segment1.transform.distance_to(holding_point);
        float segment1_angle_offset = 
            geometry2d.Triangles.get_angle_by_lengths(
                segment1.length,
                distance_to_aim,
                segment2.length
            );
        if (float.IsNaN(segment1_angle_offset)) {
            UnityEngine.Debug.Assert(true, "can't hold onto the point");
            return false;    
        }
        segment1.set_direction(
            segment1.transform.degrees_to(holding_point)+
            (folding_direction * segment1_angle_offset )
        );


        segment2.direct_to(holding_point);
        return true;
    }
    
    
}
}