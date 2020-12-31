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
    IChild_of_group
{
    public Segment segment1; //beginning (root)
    public Segment segment2; //ending (leaf)
    
    public Transform tip;

    [HideInInspector]
    public unity.geometry2d.Side folding_direction; //1 of -1
    [SerializeField]
    
    public Transform parent {
        get { return this.transform.parent;}
        set { this.transform.parent = value; }
    }
    public Vector2 local_position {
        get { return this.transform.localPosition; }
        set { this.transform.localPosition = value; }
    }
    
    public GameObject main_object {
        get { return gameObject; }
    }
    
    protected virtual Limb2.Debug debug_limb { get; set; }


    protected virtual void Awake() {
        
    }

    protected virtual void Start() {
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
        segment1.collide_with_rotation_borders();
        segment2.collide_with_rotation_borders();
    }

    public bool at_desired_rotation() {
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
                segment1.absolute_length,
                distance_to_aim,
                segment2.absolute_length
            );
        bool unreacheble = false;
        if (float.IsNaN(segment1_angle_offset)) {
            debug_limb.draw_lines(Color.magenta,0.5f);
            segment1_angle_offset = 0f;
            unreacheble = true;
        }

        Quaternion segment1_rotation = degrees_to_quaternion(
            segment1.position.degrees_to(target) +
            (folding_direction *  segment1_angle_offset )
        );
        Vector3 elbow_position = segment1.transform.position + segment1.sized_tip.rotate(segment1_rotation);
        return new Directions(
            segment1_rotation,
            degrees_to_quaternion(
                elbow_position.degrees_to(target)
            ),
            unreacheble
        );
    }

    
    public void set_desired_directions_by_position(Vector2 target) {
        Directions directions = determine_directions_reaching_point(target);
        segment1.target_quaternion = directions.segment1;
        segment2.target_quaternion = directions.segment2;

    }

    public bool hold_onto_point(Vector2 target) {
        Directions directions = determine_directions_reaching_point(target);
        segment1.rotation = directions.segment1;
        segment2.rotation = directions.segment2;
        return !directions.failed;
    }
    
    
}
}