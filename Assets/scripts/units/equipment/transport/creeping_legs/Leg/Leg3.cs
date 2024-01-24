using System;
using UnityEngine;
using rvinowise.unity.extensions;
using rvinowise.contracts;

namespace rvinowise.unity.units.parts.limbs.creeping_legs {

[Serializable]
public partial class Leg3: 
ALeg,
IMirrored
{

    public Creeping_leg_segment coxa;
    public Creeping_leg_segment femur {
        get { return segment1 as creeping_legs.Creeping_leg_segment;}
        set { segment1 = value; }
    }
    public Creeping_leg_segment tibia {
        get { return segment2 as creeping_legs.Creeping_leg_segment;}
        set { segment2 = value; }
    }
    
    #region Leg

    protected void Awake() {
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
                    coxa.target_rotation,
                    coxa.rotation
                ) <= allowed_angle
            )&&
            (
                Quaternion.Angle(
                    femur.target_rotation,
                    femur.rotation
                ) <= allowed_angle
            )&&
            (
                Quaternion.Angle(
                    tibia.target_rotation,
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
        is_up = false;
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
            femur.debug_draw_line(Color.red,1);
            return true;
        }
        if (!tibia.is_within_span())
        {
            tibia.debug_draw_line(Color.red,1);
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

    public IMirrored create_mirrored()
    {
        Leg3 dst = GameObject.Instantiate(this).GetComponent<Leg3>();
        dst.local_position = new Vector2(
            local_position.x,
            -local_position.y
        );
        dst.coxa.mirror_from(coxa);
        dst.femur.mirror_from(femur);
        dst.tibia.mirror_from(tibia);

        dst.optimal_relative_position_standing_transform.localPosition =
            optimal_relative_position_standing_transform.localPosition.mirror();

        return dst;
    }

        

    #endregion

   
    #region debug
    public override void draw_positions() {
        const float sphere_size = 0.1f;
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(holding_point, sphere_size);
        
        Gizmos.color = Color.white;
        Gizmos.DrawSphere(
            optimal_position_standing, sphere_size);
        
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(
            optimal_position, sphere_size);
        
    }

    public override void draw_directions(
        Color color,
        float time = 0.1f
    ) {
        //float time=0.1f;
        UnityEngine.Debug.DrawLine(
            coxa.position, 
            coxa.tip,
            color,
            time
        );
        UnityEngine.Debug.DrawLine(
            femur.position, 
            femur.tip,
            color,
            time
        );
        
        UnityEngine.Debug.DrawLine(
            tibia.position,
            tibia.tip,
            color,
            time
        );
    }
    public override void draw_desired_directions() {
        float time=0.1f;
        UnityEngine.Debug.DrawLine(
            coxa.position, 
            coxa.desired_tip,
            Color.cyan,
            time
        );
        UnityEngine.Debug.DrawLine(
            coxa.desired_tip, 
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
