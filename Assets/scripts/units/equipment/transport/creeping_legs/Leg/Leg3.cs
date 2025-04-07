using System;
using UnityEngine;
using rvinowise.unity.extensions;
using rvinowise.contracts;

namespace rvinowise.unity {

[Serializable]
public class Leg3: 
    ALeg
{

    public Segment coxa;
    public Segment femur {
        get { return segment1;}
        set { segment1 = value; }
    }
    public Segment tibia {
        get { return segment2;}
        set { segment2 = value; }
    }
    
    #region Leg

    protected override void Awake() {
        base.Awake();
        check_components();
    }

    private void check_components() {
        Contract.Ensures(optimal_relative_position_standing_transform != null);
    }



    public Vector2 deduce_optimal_aim() {
        float full_length = femur.tip.magnitude + tibia.tip.magnitude;
        return local_position.normalized * full_length/2;
    }

    public override bool has_reached_aim() {
        float allowed_angle = 5f;
        if (
            (
                Quaternion.Angle(
                    coxa.get_target_rotation(),
                    coxa.rotation
                ) <= allowed_angle
            )&&
            (
                Quaternion.Angle(
                    femur.get_target_rotation(),
                    femur.rotation
                ) <= allowed_angle
            )&&
            (
                Quaternion.Angle(
                    tibia.get_target_rotation(),
                    tibia.rotation
                ) <= allowed_angle
            )
         ) 
        {
            return true;
        }
        return false;
    }
    private bool is_touching_point(Vector2 aim) {
        if (tibia.tip.within_square_from(aim, 0.1f)) {
            return true;
        }
        return false;
    }
    
    public override void put_down() {
        up = false;
        holding_point = tibia.tip;
    }

    /* it's time to reposition */
    public override bool is_twisted_uncomfortably() {
        Vector2 diff_with_optimal_point = 
            optimal_position - 
            (Vector2)tibia.tip;

        
        if (diff_with_optimal_point.magnitude > reposition_distance) {
            return true;
        } 

        return false;
    }
    /* physically can't be in this position */
    public override bool is_twisted_badly() {
        if (!femur.is_within_span())
        {
            return true;
        }
        if (!tibia.is_within_span())
        {
            return true;
        }
        
        return false;
    }

    

    public override void move_segments_towards_desired_direction() {
        coxa.rotate_to_desired_direction();
        femur.rotate_to_desired_direction();
        tibia.rotate_to_desired_direction();
    }

   
    /* slower calculation but precise */
    public override bool hold_onto_ground() {
        return hold_onto_point(holding_point);
    }

    public bool is_valid() {
        // this way it can be deleted from the editor when debugging
        if (!femur.gameObject) {
            return false;
        }
        if (!tibia.gameObject) {
            return false;
        }
        return true;
    }

    public override Vector2 get_end_position_from_angles(
        Quaternion femur_rotation,
        Quaternion tibia_rotation
        ) 
    {
        Vector2 position =
            (Vector2) (femur_rotation * femur.tip) +
            (Vector2) (
                tibia_rotation *
                femur_rotation *
                tibia.tip
            );
        return position;
    }


    public override void set_desired_position(Vector2 in_optimal_position) {
        optimal_position = in_optimal_position;
        coxa.target_degree =
            coxa.position.degrees_to(in_optimal_position);
        base.set_desired_directions_by_position(optimal_position);
    }

    

        

    #endregion

    #region IAttaker
    
    #region IAttacker
    
    public override void attack(Transform target, Action on_completed = null) {
        on_completed?.Invoke();
    }
    
    #endregion
    
    #endregion
    
   
    #region debug
    public override void draw_positions() {
        const float sphere_size = 0.1f;
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(holding_point, sphere_size);
        
        Gizmos.color = Color.white;
        Gizmos.DrawSphere(
            get_optimal_position_standing(), sphere_size);
        
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(
            optimal_position, sphere_size);
        
    }

    public override void draw_directions(
        Color color
    ) {
        Gizmos.color = color;
        //float time=0.1f;
        UnityEngine.Gizmos.DrawLine(
            coxa.position, 
            coxa.tip
        );
        UnityEngine.Gizmos.DrawLine(
            femur.position, 
            femur.tip
        );
        
        UnityEngine.Gizmos.DrawLine(
            tibia.position,
            tibia.tip
            
        );
    }
    public override void draw_desired_directions() {
        Gizmos.color=Color.cyan;
        UnityEngine.Gizmos.DrawLine(
            coxa.position, 
            coxa.desired_tip
        );
        UnityEngine.Gizmos.DrawLine(
            coxa.desired_tip, 
            segment1.desired_tip
        );
        
        UnityEngine.Gizmos.DrawLine(
            segment1.desired_tip,
            segment2.desired_tip
        );
        
    }
    #endregion
} 





}
