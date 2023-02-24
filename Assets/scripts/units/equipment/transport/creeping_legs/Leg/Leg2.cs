using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rvinowise.unity.extensions;

using rvinowise.unity.geometry2d;
using static rvinowise.unity.geometry2d.Directions;
using rvinowise.unity.units;
using rvinowise.contracts;

namespace rvinowise.unity.units.parts.limbs.creeping_legs {

[Serializable]
public partial class Leg2: 
ALeg,
IMirrored
{
    /* constant characteristics */
    public Creeping_leg_segment femur {
        get { return segment1 as creeping_legs.Creeping_leg_segment;}
        set { segment1 = value; }
    }
    public Creeping_leg_segment tibia {
        get { return segment2 as creeping_legs.Creeping_leg_segment;}
        set { segment2 = value; }
    }


    protected void Awake() {

        check_components();
    }

    private void check_components() {
        Contract.Ensures(optimal_relative_position_standing_transform != null);
    }



    #region ILeg
    
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

    public override bool is_twisted_uncomfortably() {
        Vector2 diff_with_optimal_point = 
            optimal_position - 
            (Vector2)tibia.tip;
        
        if (diff_with_optimal_point.magnitude > reposition_distance) {
            return true;
        } 

        return false;
    }

    public override bool hold_onto_ground() {
        return hold_onto_point(holding_point);
    }

    
    

    #endregion //ILeg

    public IMirrored create_mirrored()
    {
        Leg2 dst = GameObject.Instantiate(this).GetComponent<Leg2>();
        dst.local_position = new Vector2(
            local_position.x,
            -local_position.y
        );
        dst.femur.mirror_from(femur);
        dst.tibia.mirror_from(tibia);

        dst.optimal_relative_position_standing_transform.localPosition =
            optimal_relative_position_standing_transform.localPosition.mirror();

        return dst;
    }

    #region debug
    public override void draw_positions() {
        const float sphere_size = 0.1f;
        Gizmos.color = new Color(1f,1f,0f,0.4f);
        Gizmos.DrawSphere(holding_point, sphere_size);
        
        Gizmos.color = new Color(1f,1f,1f,0.4f);
        Gizmos.DrawSphere(
            optimal_position_standing, sphere_size);
        
        Gizmos.color = new Color(0f,0f,1f,0.4f);
        Gizmos.DrawSphere(
            optimal_position, sphere_size);
        
    }
    #endregion
    
} 





}
