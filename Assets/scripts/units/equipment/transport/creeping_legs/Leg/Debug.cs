using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rvinowise.unity.extensions;

using rvinowise;
using rvinowise.unity.geometry2d;


namespace rvinowise.unity.units.parts.limbs.creeping_legs {

public partial class Leg : Limb2 {
    
    
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
    
    public new class Debug: Limb2.Debug {
        private readonly Leg leg;

        protected override Limb2 limb2 {
            get { return leg; }
        }

        public Debug(Leg parent):base(parent) {
            leg = parent;
        }

        public void draw_positions() {
            if (debug_off) {
                return;
            }
            /*if (name != "left_hind_leg") {
                return;
            }*/
            /*Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(
                leg.segment2.transform.TransformPoint(leg.segment2.tip), sphere_size);*/
            
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(leg.holding_point, sphere_size);
            
            Gizmos.color = Color.white;
            Gizmos.DrawSphere(
                leg.optimal_position_standing, sphere_size);
            
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(
                leg.optimal_position, sphere_size);
           
        }
    }
    public Leg.Debug debug {
        get { return _debug; }
        private set { _debug = value; }
    }
    private Leg.Debug _debug;

    protected override Limb2.Debug debug_limb {
        get { return _debug; }
    }
    
}
}