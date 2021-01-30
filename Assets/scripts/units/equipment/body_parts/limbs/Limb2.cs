using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using rvinowise.unity.geometry2d;
using UnityEngine;
using rvinowise.unity.extensions;

using rvinowise;
using rvinowise.contracts;
using UnityEngine.Assertions;
using static rvinowise.unity.geometry2d.Directions;


namespace rvinowise.unity.units.parts.limbs {

[Serializable]
public partial class Limb2:
    MonoBehaviour, 
    ILimb,
    IChild_of_group
{
    public Segment segment1; //beginning (root)
    public Segment segment2; //ending (leaf)
    
    [HideInInspector]
    public unity.geometry2d.Side folding_direction; //1 of -1
    [SerializeField]
    
    public Vector2 local_position {
        get { return this.transform.localPosition; }
        set { this.transform.localPosition = value; }
    }
    
    
    protected virtual Limb2.Debug debug_limb { get; set; }


    protected virtual void Awake() {
        
    }

    protected virtual void Start() {
        init_folding_direction();
    }

    void FixedUpdate() {
        preserve_possible_rotations();
    }

    public void init_folding_direction() {
        folding_direction = segment2.possible_span.side_of_bigger_rotation().mirror();
        Contract.Ensures(
            folding_direction != Side.NONE,
            "rotation span of Segment #2 should define folding direction of the limb"
        );
    }
    
    public virtual void rotate_to_desired_directions() {
        segment1.rotate_to_desired_direction();
        segment2.rotate_to_desired_direction();
    }
    public virtual void jump_to_desired_directions() {
        segment1.jump_to_desired_direction();
        segment2.jump_to_desired_direction();
    }
    
    public virtual void preserve_possible_rotations() {
        segment1.preserve_possible_rotations();
        segment2.preserve_possible_rotations();
    }

    public virtual bool at_desired_rotation() {
        return (
            this.segment1.at_desired_rotation() &&
            this.segment2.at_desired_rotation()
        );

    }
    
    private struct Directions {
        public Quaternion segment1;
        public Quaternion segment2;
        public bool failed;

        public Directions(
            Quaternion in1,
            Quaternion in2,
            bool in_unreacheble = false
        ) {
            segment1 = in1;
            segment2 = in2;
            failed = in_unreacheble;
        }
    }

    private Directions determine_directions_reaching_point(Vector2 target) {
        Contract.Requires((folding_direction != Side.NONE) && (folding_direction != null),
            "folding_direction is needed for bending the limb"
        );
        float distance_to_aim = segment1.position.distance_to(target);
        float segment1_angle_offset = 
            unity.geometry2d.Triangles.get_angle_by_lengths(
                segment1.length,
                distance_to_aim,
                segment2.length
            );
        bool unreacheble = false;
        if (float.IsNaN(segment1_angle_offset)) {
            draw_directions(Color.magenta,0.5f);
            segment1_angle_offset = 0f;
            unreacheble = true;
        }

        Quaternion segment1_rotation = degrees_to_quaternion(
            segment1.position.degrees_to(target) +
            (folding_direction *  segment1_angle_offset )
        );
        Vector3 elbow_position = segment1.position + (segment1_rotation * segment1.localTip);
        return new Directions(
            segment1_rotation,
            degrees_to_quaternion(
                elbow_position.degrees_to(target)
            ),
            unreacheble
        );
    }

    
    public void set_desired_directions_by_position(Vector2 target) {
        if (this.name == "leg_l_f") {
            bool test = true;
        }
        Directions directions = determine_directions_reaching_point(target);
        segment1.target_rotation = directions.segment1;
        segment2.target_rotation = directions.segment2;

    }

    public bool hold_onto_point(Vector2 target) {
        Directions directions = determine_directions_reaching_point(target);
        segment1.rotation = directions.segment1;
        segment2.rotation = directions.segment2;
        return !directions.failed;
    }

    protected void set_relative_mirrored_target_direction(
        Segment in_segment,
        float in_degrees
    ) {
        if (folding_direction == Side.RIGHT) {
            in_degrees = -in_degrees;
        }
        in_segment.target_rotation = geometry2d.Directions.degrees_to_quaternion(in_degrees);
    }

 
    public virtual bool is_twisted_badly() {
        if (!segment1.is_within_span())
        {
            segment1.debug_draw_line(Color.red,1);
            return true;
        }
        if (!segment2.is_within_span())
        {
            if (this.name == "leg_l_f") {
                bool test = segment2.is_within_span();
            }
            segment2.debug_draw_line(Color.red,1);
            return true;
        }
        
        return false;
    }

    public virtual void move_segments_towards_desired_direction() {
        segment1.rotate_to_desired_direction();
        segment2.rotate_to_desired_direction();
    }


    public virtual Vector2 get_end_position_from_angles(
        Quaternion segment1_rotation,
        Quaternion segment2_rotation
        ) 
    {
        Vector2 position =
            (Vector2) (segment1_rotation * segment1.localTip) +
            (Vector2) (
                segment2_rotation *
                segment1_rotation *
                segment2.localTip
            );
        return position;
    }

    public virtual bool has_reached_aim() {
        float allowed_angle = 5f;
        if (
            (
                Quaternion.Angle(
                    segment1.target_rotation,
                    segment1.rotation
                ) <= allowed_angle
            )&&
            (
                Quaternion.Angle(
                    segment2.target_rotation,
                    segment2.rotation
                ) <= allowed_angle
            )
         ) 
        {
            return true;
        }
        return false;
    }

    #region debug
    
    public virtual void draw_directions(
        Color color,
        float time = 0.1f
    ) {
        //float time=0.1f;
        UnityEngine.Debug.DrawLine(
            segment1.position, 
            segment1.tip,
            color,
            time
        );
        
        UnityEngine.Debug.DrawLine(
            segment2.position,
            segment2.tip,
            color,
            time
        );
    }
    public virtual void draw_desired_directions() {
        float time=0.1f;
        UnityEngine.Debug.DrawLine(
            segment1.position, 
            segment1.desired_tip,
            Color.cyan,
            time
        );
        
        UnityEngine.Debug.DrawLine(
            segment1.desired_tip,
            segment2.desired_tip,
            Color.cyan,
            time
        );
        
    }
    #endregion
}
}