using System;
using rvinowise.unity.geometry2d;
using UnityEngine;
using rvinowise.unity.extensions;
using rvinowise.contracts;
using rvinowise.unity.actions;
using TMPro;
using UnityEditor;
using static rvinowise.unity.geometry2d.Directions;
using Action = rvinowise.unity.actions.Action;


namespace rvinowise.unity {

[Serializable]
public class Limb2:
    MonoBehaviour, 
    ILimb,
    IChild_of_group
{
    public Segment segment1; //beginning (root)
    public Segment segment2; //ending (leaf)
    
    public Side_type folding_side; //1 of -1
    public unity.geometry2d.Side_type side => folding_side; // left or right arm?

    
    public Vector2 local_position {
        get { return this.transform.localPosition; }
        set { this.transform.localPosition = value; }
    }


    public Segment get_root_segment() => segment1;

    protected virtual void Awake() {
        actor = GetComponent<Actor>();
    }


    public virtual void rotate_to_desired_directions() {
        segment1.rotate_to_desired_direction();
        segment2.rotate_to_desired_direction();
    }
    public virtual void jump_to_desired_directions() {
        segment1.jump_to_desired_direction();
        segment2.jump_to_desired_direction();
    }
    

    public virtual bool at_desired_rotation() {
        return 
            segment1.at_desired_rotation() &&
            segment2.at_desired_rotation()
        ;

    }
    
    public struct Directions {
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

    public Directions determine_directions_reaching_point(Vector2 target) {
        Contract.Requires((folding_side != Side_type.NONE),
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
            draw_directions_debug(Color.magenta);
            segment1_angle_offset = 0f;
            unreacheble = true;
        }

        Quaternion segment1_rotation = degrees_to_quaternion(
            segment1.position.degrees_to(target) +
            Side.turn_degrees(folding_side, segment1_angle_offset )
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
        Directions directions = determine_directions_reaching_point(target);
        segment1.set_target_rotation(directions.segment1);
        segment2.set_target_rotation(directions.segment2);

    }

    public bool hold_onto_point(Vector2 target) {
        Directions directions = determine_directions_reaching_point(target);
        segment1.transform.rotation = directions.segment1;
        segment2.transform.rotation = directions.segment2;
        return !directions.failed;
    }

    public void set_relative_mirrored_target_direction(
        Segment in_segment,
        float in_degrees
    ) {
        if (folding_side == Side_type.RIGHT) {
            in_degrees = -in_degrees;
        }
        in_segment.target_direction_relative = true;
        in_segment.set_target_rotation(geometry2d.Directions.degrees_to_quaternion(in_degrees));
    }

 
    public virtual bool is_twisted_badly() {
        if (!segment1.is_within_span())
        {
            return true;
        }
        if (!segment2.is_within_span())
        {
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
                    segment1.get_target_rotation(),
                    segment1.rotation
                ) <= allowed_angle
            )&&
            (
                Quaternion.Angle(
                    segment2.get_target_rotation(),
                    segment2.rotation
                ) <= allowed_angle
            )
         ) 
        {
            return true;
        }
        return false;
    }

    public float get_reaching_distance() {
        return (segment1.length + segment2.length);
    }

    #region debug
    
    public virtual void draw_directions(
        Color color
    ) {
        Gizmos.color = color;
        try {
            UnityEngine.Gizmos.DrawLine(
                segment1.position,
                segment1.tip
            );

            UnityEngine.Gizmos.DrawLine(
                segment2.position,
                segment2.tip
            );
        }
        catch (NullReferenceException exc) {
            UnityEngine.Debug.LogError("Exception: "+exc.Message);
        }
    }
    public virtual void draw_directions_debug(
        Color color
    ) {
        Gizmos.color = color;
        try {
            UnityEngine.Debug.DrawLine(
                segment1.position,
                segment1.tip
            );

            UnityEngine.Debug.DrawLine(
                segment2.position,
                segment2.tip
            );
        }
        catch (NullReferenceException exc) {
            UnityEngine.Debug.LogError("Exception: "+exc.Message);
        }
    }
    public virtual void draw_desired_directions() {
        Gizmos.color = Color.cyan;
        
        UnityEngine.Gizmos.DrawLine(
            segment1.position, 
            segment1.desired_tip
        );
        
        UnityEngine.Gizmos.DrawLine(
            segment1.desired_tip,
            segment2.desired_tip
        );
        
    }
    
#if UNITY_EDITOR
    protected virtual void OnDrawGizmos() {
        draw_desired_directions();
        draw_directions(Color.white);

        // if (current_action != null) {
        //     GUIStyle myStyle = new GUIStyle {
        //         fontSize = 13
        //         //fontStyle = FontStyle.Bold
        //     };
        //     myStyle.normal.textColor = Color.green;
        //
        //     Handles.Label(transform.position, current_action.ToString(),myStyle);
        // }
    }
#endif
    
    #endregion


    public Actor actor { get; set; }

    public virtual void on_lacking_action() {
    }


}
}