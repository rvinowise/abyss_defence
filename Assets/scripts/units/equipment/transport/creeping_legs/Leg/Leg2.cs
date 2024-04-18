using System;
using UnityEngine;
using rvinowise.unity.extensions;
using rvinowise.contracts;
using rvinowise.unity.actions;
using Action = System.Action;


namespace rvinowise.unity {

[Serializable]
public class Leg2: 
    ALeg
{
    /* constant characteristics */
    public Segment femur {
        get { return segment1;}
        set { segment1 = value; }
    }
    public Segment tibia {
        get { return segment2;}
        set { segment2 = value; }
    }


    #region ILeg
    
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

    public override bool is_twisted_uncomfortably() {
        Vector2 diff_with_optimal_point = 
            optimal_position - 
            tibia.tip;
        
        if (diff_with_optimal_point.magnitude > reposition_distance) {
            return true;
        } 

        return false;
    }

    public override bool hold_onto_ground() {
        return hold_onto_point(holding_point);
    }

    
    

    #endregion ILeg

    
    #region IAttacker
    
    public override void attack(Transform target, Action on_completed = null) {
        Leg2_grab_target.create(
            this,
            target
        ).set_on_completed(on_completed)
        .start_as_root(action_runner);
    }
    
    #endregion
    
    #region debug
    public override void draw_positions() {
        const float sphere_size = 0.1f;
        Gizmos.color = new Color(1f,1f,0f,0.4f);
        Gizmos.DrawSphere(holding_point, sphere_size);
        
        Gizmos.color = new Color(1f,1f,1f,0.4f);
        Gizmos.DrawSphere(
            get_optimal_position_standing(), sphere_size);
        
        Gizmos.color = new Color(0f,0f,1f,0.4f);
        Gizmos.DrawSphere(
            optimal_position, sphere_size);
        
    }

    protected override void OnDrawGizmos() {
        base.OnDrawGizmos();
        draw_positions();
        if (
            (is_twisted_badly())
            ||
            (determine_directions_reaching_point(holding_point).failed)
        ) 
        {
            draw_directions(Color.red);
        } 
    }

    #endregion
    
    #region optimization
    /* faster calculation but not precise */
    public void hold_onto_ground_FAST(Vector2 holding_point) {
        tibia.direct_to(holding_point);
        femur.set_direction(
            femur.transform.degrees_to(holding_point)+
            (90f-femur.transform.sqr_distance_to(holding_point)*1440f)
        );
    }
    #endregion
    
} 





}
